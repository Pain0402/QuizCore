using System.Collections.Generic;
using QuizCore.Domain.Enums;

namespace QuizCore.Domain.Entities;

public class Question
{
    public int Id { get; set; }
    
    public int SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;
    
    public string Topic { get; set; } = string.Empty;
    
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    public QuestionDifficulty Difficulty { get; set; }
    public QuestionType Type { get; set; }
    public string Content { get; set; } = string.Empty;

    public int CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;

    public bool IsDeleted { get; set; } = false;

    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    public ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();
    public ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();
}
