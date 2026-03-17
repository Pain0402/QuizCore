namespace QuizCore.Domain.Entities;

public class ExamAttempt
{
    public int Id { get; set; }

    public int ExamId { get; set; }
    public Exam Exam { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime StartedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }

    public string Status { get; set; } = "in_progress"; // in_progress | completed | expired
    public decimal Score { get; set; } = 0;

    public bool IsDeleted { get; set; } = false;

    public ICollection<AttemptDetail> AttemptDetails { get; set; } = new List<AttemptDetail>();
    public ICollection<ExamLog> ExamLogs { get; set; } = new List<ExamLog>();
}
