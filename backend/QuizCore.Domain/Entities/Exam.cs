namespace QuizCore.Domain.Entities;

public class Exam
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public int SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;

    public int Duration { get; set; } // minutes
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int MaxAttempts { get; set; } = 1;
    public bool ShuffleQuestions { get; set; } = true;
    public bool ShuffleAnswers { get; set; } = true;

    public decimal TotalMark { get; set; } = 100;
    public decimal PassMark { get; set; } = 50;

    public int CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;

    public bool IsDeleted { get; set; } = false;

    public ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();
    public ICollection<ExamAttempt> ExamAttempts { get; set; } = new List<ExamAttempt>();
}
