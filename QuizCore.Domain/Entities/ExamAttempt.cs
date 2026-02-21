using System;
using System.Collections.Generic;
using QuizCore.Domain.Enums;

namespace QuizCore.Domain.Entities;

public class ExamAttempt
{
    public int Id { get; set; }
    
    public int ExamId { get; set; }
    public Exam Exam { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime StartTime { get; set; }
    public DateTime? SubmitTime { get; set; }
    
    public AttemptStatus Status { get; set; } = AttemptStatus.InProgress;
    public float? TotalScore { get; set; }

    public bool IsDeleted { get; set; } = false;

    public ICollection<AttemptDetail> AttemptDetails { get; set; } = new List<AttemptDetail>();
    public ICollection<ExamLog> ExamLogs { get; set; } = new List<ExamLog>();
}
