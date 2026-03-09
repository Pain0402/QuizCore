namespace QuizCore.Application.DTOs.Attempts;

public class StartAttemptResponse
{
    public int AttemptId { get; set; }
    public int ExamId { get; set; }
    public string ExamTitle { get; set; } = string.Empty;
    public int Duration { get; set; } // seconds
    public DateTime StartedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public List<AttemptQuestionDto> Questions { get; set; } = [];
}

public class AttemptQuestionDto
{
    public int QuestionId { get; set; }
    public int Order { get; set; }
    public string Content { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public List<AttemptAnswerDto> Answers { get; set; } = [];
}

public class AttemptAnswerDto
{
    public int AnswerId { get; set; }
    public string Content { get; set; } = string.Empty;
}
