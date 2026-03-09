namespace QuizCore.Application.DTOs.Attempts;

public class AttemptResultResponse
{
    public int AttemptId { get; set; }
    public int ExamId { get; set; }
    public string ExamTitle { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public decimal TotalMark { get; set; }
    public decimal PassMark { get; set; }
    public bool IsPassed { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public int DurationSeconds { get; set; }
    public List<QuestionReviewDto> Review { get; set; } = [];
}

public class QuestionReviewDto
{
    public int QuestionId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public List<AnswerReviewDto> Answers { get; set; } = [];
}

public class AnswerReviewDto
{
    public int AnswerId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public bool WasSelected { get; set; }
}
