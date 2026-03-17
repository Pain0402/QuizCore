using System.Collections.Generic;

namespace QuizCore.Application.DTOs.Questions;

/// <summary>
/// DTO nhận từ frontend – dùng tên field khớp với Vue form
/// </summary>
public class QuestionCreateDto
{
    public int SubjectId { get; set; } = 1; // default subject nếu không có
    public string Topic { get; set; } = string.Empty;
    public string Difficulty { get; set; } = "Medium";
    public string QuestionType { get; set; } = "SingleChoice";
    public string Content { get; set; } = string.Empty;
    public List<AnswerCreateDto> Answers { get; set; } = new();
}

public class AnswerCreateDto
{
    public string Content { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}
