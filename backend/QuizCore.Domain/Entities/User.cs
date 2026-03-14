using System;
using System.Collections.Generic;
using QuizCore.Domain.Enums;

namespace QuizCore.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public UserRole Role { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserClass> UserClasses { get; set; } = new List<UserClass>();
    public ICollection<ExamAttempt> ExamAttempts { get; set; } = new List<ExamAttempt>();
    public ICollection<Exam> CreatedExams { get; set; } = new List<Exam>();
    public ICollection<Question> CreatedQuestions { get; set; } = new List<Question>();
}
