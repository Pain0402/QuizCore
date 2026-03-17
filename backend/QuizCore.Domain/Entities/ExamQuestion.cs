namespace QuizCore.Domain.Entities;

public class ExamQuestion
{
    public int ExamId { get; set; }
    public Exam Exam { get; set; } = null!;

    public int QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    public int Order { get; set; } = 0;
    public float ScoreWeight { get; set; } = 1.0f;
}
