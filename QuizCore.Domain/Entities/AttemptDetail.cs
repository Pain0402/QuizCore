namespace QuizCore.Domain.Entities;

public class AttemptDetail
{
    public long Id { get; set; }
    
    public int AttemptId { get; set; }
    public ExamAttempt Attempt { get; set; } = null!;

    public int QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    public int? SelectedAnswerId { get; set; }
    public Answer? SelectedAnswer { get; set; }

    public string? TextAnswer { get; set; }
    public bool? IsCorrect { get; set; }
}
