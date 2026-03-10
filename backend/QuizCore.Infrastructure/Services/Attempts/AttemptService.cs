using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using QuizCore.Application.DTOs.Attempts;
using QuizCore.Application.Interfaces.Attempts;
using QuizCore.Domain.Entities;
using QuizCore.Infrastructure.Data;

namespace QuizCore.Infrastructure.Services.Attempts;

public class AttemptService(AppDbContext db, IDistributedCache cache) : IAttemptService
{
    private static string RedisKey(int attemptId) => $"attempt:{attemptId}:answers";
    private static string TimerKey(int attemptId) => $"attempt:{attemptId}:start";

    // ─────────────────────────────────────────────
    public async Task<StartAttemptResponse> StartExamAsync(int examId, int userId)
    {
        var exam = await db.Exams
            .Include(e => e.ExamQuestions)
                .ThenInclude(eq => eq.Question)
                    .ThenInclude(q => q.Answers)
            .FirstOrDefaultAsync(e => e.Id == examId)
            ?? throw new Exception("Đề thi không tồn tại");

        // Check max attempts
        var attemptCount = await db.ExamAttempts
            .CountAsync(a => a.ExamId == examId && a.UserId == userId && !a.IsDeleted);
        if (attemptCount >= exam.MaxAttempts)
            throw new InvalidOperationException($"Bạn đã hết số lần thi ({exam.MaxAttempts} lần)");

        // Check if there's an active attempt
        var activeAttempt = await db.ExamAttempts
            .FirstOrDefaultAsync(a => a.ExamId == examId && a.UserId == userId
                                   && a.Status == "in_progress" && !a.IsDeleted);
        if (activeAttempt is not null)
            return await BuildStartResponse(exam, activeAttempt);

        // Create new attempt
        var attempt = new ExamAttempt
        {
            ExamId = examId,
            UserId = userId,
            StartedAt = DateTime.UtcNow,
            Status = "in_progress"
        };
        db.ExamAttempts.Add(attempt);
        await db.SaveChangesAsync();

        // Save start time in Redis for timer calculation
        var opts = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(exam.Duration * 60 + 300)
        };
        await cache.SetStringAsync(TimerKey(attempt.Id),
            attempt.StartedAt.ToString("o"), opts);

        return await BuildStartResponse(exam, attempt);
    }

    private static Task<StartAttemptResponse> BuildStartResponse(Exam exam, ExamAttempt attempt)
    {
        var durationSeconds = exam.Duration * 60;
        var questions = exam.ExamQuestions
            .OrderBy(eq => eq.Order)
            .Select(eq => new AttemptQuestionDto
            {
                QuestionId = eq.Question.Id,
                Order = eq.Order,
                Content = eq.Question.Content,
                QuestionType = eq.Question.QuestionType.ToString(),
                Answers = eq.Question.Answers.Select(a => new AttemptAnswerDto
                {
                    AnswerId = a.Id,
                    Content = a.Content
                }).ToList()
            }).ToList();

        return Task.FromResult(new StartAttemptResponse
        {
            AttemptId = attempt.Id,
            ExamId = exam.Id,
            ExamTitle = exam.Title,
            Duration = durationSeconds,
            StartedAt = attempt.StartedAt,
            ExpiresAt = attempt.StartedAt.AddSeconds(durationSeconds),
            Questions = questions
        });
    }

    // ─────────────────────────────────────────────
    public async Task AutoSaveAsync(int attemptId, int userId, AutoSaveRequest request)
    {
        var attempt = await GetActiveAttemptAsync(attemptId, userId);
        var key = RedisKey(attemptId);

        // Load current answers from Redis
        var existing = await LoadAnswersFromRedis(key);
        existing[request.QuestionId] = request.SelectedAnswerIds;

        var exam = await db.Exams.FindAsync(attempt.ExamId)!;
        var opts = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds((exam!.Duration * 60) + 300)
        };
        await cache.SetStringAsync(key, JsonSerializer.Serialize(existing), opts);
    }

    // ─────────────────────────────────────────────
    public async Task<AttemptProgressResponse> GetProgressAsync(int attemptId, int userId)
    {
        var attempt = await GetActiveAttemptAsync(attemptId, userId);
        var exam = await db.Exams.FindAsync(attempt.ExamId)!;
        var saved = await LoadAnswersFromRedis(RedisKey(attemptId));

        var elapsed = (int)(DateTime.UtcNow - attempt.StartedAt).TotalSeconds;
        var remaining = Math.Max(0, exam!.Duration * 60 - elapsed);

        return new AttemptProgressResponse
        {
            AttemptId = attemptId,
            SavedAnswers = saved,
            SecondsRemaining = remaining
        };
    }

    // ─────────────────────────────────────────────
    public async Task<AttemptResultResponse> SubmitAsync(int attemptId, int userId)
    {
        var attempt = await GetActiveAttemptAsync(attemptId, userId);

        // Get answers from Redis (latest autosave)
        var savedAnswers = await LoadAnswersFromRedis(RedisKey(attemptId));

        // Persist AttemptDetails to DB
        var exam = await db.Exams
            .Include(e => e.ExamQuestions).ThenInclude(eq => eq.Question).ThenInclude(q => q.Answers)
            .FirstOrDefaultAsync(e => e.Id == attempt.ExamId)!;

        int correctCount = 0;
        foreach (var eq in exam!.ExamQuestions)
        {
            var question = eq.Question;
            savedAnswers.TryGetValue(question.Id, out var selectedIds);
            selectedIds ??= [];

            var correctIds = question.Answers.Where(a => a.IsCorrect).Select(a => a.Id).ToHashSet();
            var isCorrect = correctIds.SetEquals(selectedIds.ToHashSet());
            if (isCorrect) correctCount++;

            foreach (var answerId in selectedIds)
            {
                db.AttemptDetails.Add(new AttemptDetail
                {
                    AttemptId = attemptId,
                    QuestionId = question.Id,
                    AnswerId = answerId,
                    IsCorrect = isCorrect
                });
            }

            // Record empty answer too (for unanswered questions)
            if (!selectedIds.Any())
            {
                db.AttemptDetails.Add(new AttemptDetail
                {
                    AttemptId = attemptId,
                    QuestionId = question.Id,
                    AnswerId = null,
                    IsCorrect = false
                });
            }
        }

        // Calculate score
        var totalQuestions = exam.ExamQuestions.Count;
        var score = totalQuestions > 0
            ? Math.Round((decimal)correctCount / totalQuestions * exam.TotalMark, 2)
            : 0;

        attempt.Score = score;
        attempt.SubmittedAt = DateTime.UtcNow;
        attempt.Status = "completed";
        await db.SaveChangesAsync();

        // Clean up Redis
        await cache.RemoveAsync(RedisKey(attemptId));
        await cache.RemoveAsync(TimerKey(attemptId));

        return BuildResult(attempt, exam, correctCount, totalQuestions);
    }

    // ─────────────────────────────────────────────
    public async Task<AttemptResultResponse> GetResultAsync(int attemptId, int userId)
    {
        var attempt = await db.ExamAttempts
            .FirstOrDefaultAsync(a => a.Id == attemptId && a.UserId == userId && !a.IsDeleted)
            ?? throw new Exception("Không tìm thấy lượt thi");

        if (attempt.Status != "completed")
            throw new InvalidOperationException("Lượt thi chưa hoàn thành");

        var exam = await db.Exams
            .Include(e => e.ExamQuestions).ThenInclude(eq => eq.Question).ThenInclude(q => q.Answers)
            .FirstOrDefaultAsync(e => e.Id == attempt.ExamId)!;

        var details = await db.AttemptDetails
            .Where(d => d.AttemptId == attemptId)
            .ToListAsync();

        var correctCount = details
            .GroupBy(d => d.QuestionId)
            .Count(g => g.Any(d => d.IsCorrect));

        return BuildResult(attempt, exam!, correctCount, exam!.ExamQuestions.Count, details);
    }

    // ─────────────────────────────────────────────
    private static AttemptResultResponse BuildResult(ExamAttempt attempt, Exam exam,
        int correctCount, int totalQuestions, List<AttemptDetail>? details = null)
    {
        var isPassed = attempt.Score >= exam.PassMark;

        var review = exam.ExamQuestions.OrderBy(eq => eq.Order).Select(eq =>
        {
            var q = eq.Question;
            var selectedIds = details?
                .Where(d => d.QuestionId == q.Id)
                .Select(d => d.AnswerId)
                .ToHashSet() ?? [];

            return new QuestionReviewDto
            {
                QuestionId = q.Id,
                Content = q.Content,
                IsCorrect = q.Answers.Where(a => a.IsCorrect).Select(a => (int?)a.Id).ToHashSet()
                              .SetEquals(selectedIds),
                Answers = q.Answers.Select(a => new AnswerReviewDto
                {
                    AnswerId = a.Id,
                    Content = a.Content,
                    IsCorrect = a.IsCorrect,
                    WasSelected = selectedIds.Contains((int?)a.Id)
                }).ToList()
            };
        }).ToList();

        return new AttemptResultResponse
        {
            AttemptId = attempt.Id,
            ExamId = exam.Id,
            ExamTitle = exam.Title,
            Score = attempt.Score,
            TotalMark = exam.TotalMark,
            PassMark = exam.PassMark,
            IsPassed = isPassed,
            TotalQuestions = totalQuestions,
            CorrectAnswers = correctCount,
            StartedAt = attempt.StartedAt,
            SubmittedAt = attempt.SubmittedAt,
            DurationSeconds = exam.Duration * 60,
            Review = review
        };
    }

    // ─────────────────────────────────────────────
    private async Task<ExamAttempt> GetActiveAttemptAsync(int attemptId, int userId)
    {
        return await db.ExamAttempts
            .FirstOrDefaultAsync(a => a.Id == attemptId && a.UserId == userId
                                   && a.Status == "in_progress" && !a.IsDeleted)
            ?? throw new InvalidOperationException("Lượt thi không hợp lệ hoặc đã kết thúc");
    }

    private async Task<Dictionary<int, List<int>>> LoadAnswersFromRedis(string key)
    {
        var json = await cache.GetStringAsync(key);
        return string.IsNullOrEmpty(json)
            ? []
            : JsonSerializer.Deserialize<Dictionary<int, List<int>>>(json) ?? [];
    }
}
