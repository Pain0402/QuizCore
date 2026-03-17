using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuizCore.Application.DTOs.Questions;
using QuizCore.Application.Interfaces.Questions;
using QuizCore.Domain.Entities;
using QuizCore.Infrastructure.Data;
using QuizCore.Domain.Enums;
using System;

namespace QuizCore.Infrastructure.Services.Questions;

public class QuestionService : IQuestionService
{
    private readonly AppDbContext _context;

    public QuestionService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Map chuỗi từ frontend (SingleChoice, MultipleChoice, TrueFalse, Single, Multiple, Short)
    /// sang QuestionType enum
    /// </summary>
    private static QuestionType ParseType(string? s) => s?.ToLower() switch
    {
        "single" or "singlechoice"   => QuestionType.Single,
        "multiple" or "multiplechoice" => QuestionType.Multiple,
        "short" or "truefalse"       => QuestionType.Short,
        _                            => QuestionType.Single
    };

    /// <summary>
    /// Map chuỗi từ frontend (Easy, Medium, Hard) sang QuestionDifficulty enum
    /// </summary>
    private static QuestionDifficulty ParseDifficulty(string? s) => s?.ToLower() switch
    {
        "easy"   => QuestionDifficulty.Easy,
        "medium" => QuestionDifficulty.Medium,
        "hard"   => QuestionDifficulty.Hard,
        _        => QuestionDifficulty.Medium
    };

    public async Task<IEnumerable<Question>> GetAllAsync(int? subjectId, string? difficulty)
    {
        var query = _context.Questions
            .Include(q => q.Answers)
            .AsQueryable();

        if (subjectId.HasValue)
            query = query.Where(q => q.SubjectId == subjectId.Value);

        if (!string.IsNullOrEmpty(difficulty))
        {
            var dif = ParseDifficulty(difficulty);
            query = query.Where(q => q.Difficulty == dif);
        }

        return await query.ToListAsync();
    }

    public async Task<Question> GetByIdAsync(int id)
    {
        var question = await _context.Questions
            .Include(q => q.Answers)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (question == null) throw new Exception("Question not found");
        return question;
    }

    public async Task<Question> CreateAsync(QuestionCreateDto dto, int createdById)
    {
        var question = new Question
        {
            SubjectId    = dto.SubjectId > 0 ? dto.SubjectId : 1,
            Topic        = dto.Topic ?? string.Empty,
            Difficulty   = ParseDifficulty(dto.Difficulty),
            QuestionType = ParseType(dto.QuestionType),
            Content      = dto.Content,
            CreatedById  = createdById,
            Answers      = dto.Answers
                .Select(a => new Answer { Content = a.Content, IsCorrect = a.IsCorrect })
                .ToList()
        };

        _context.Questions.Add(question);
        await _context.SaveChangesAsync();
        return question;
    }

    public async Task UpdateAsync(int id, QuestionCreateDto dto)
    {
        var question = await GetByIdAsync(id);
        question.SubjectId    = dto.SubjectId > 0 ? dto.SubjectId : question.SubjectId;
        question.Topic        = dto.Topic ?? question.Topic;
        question.Difficulty   = ParseDifficulty(dto.Difficulty);
        question.QuestionType = ParseType(dto.QuestionType);
        question.Content      = dto.Content;

        _context.Answers.RemoveRange(question.Answers);
        question.Answers = dto.Answers
            .Select(a => new Answer { Content = a.Content, IsCorrect = a.IsCorrect })
            .ToList();

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var question = await GetByIdAsync(id);
        question.IsDeleted = true;
        await _context.SaveChangesAsync();
    }
}
