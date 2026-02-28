using System;

namespace QuizCore.Domain.Entities;

public class ExamLog
{
    public long Id { get; set; }
    
    public int AttemptId { get; set; }
    public ExamAttempt Attempt { get; set; } = null!;

    public string LogType { get; set; } = string.Empty; 
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Details { get; set; }
}
