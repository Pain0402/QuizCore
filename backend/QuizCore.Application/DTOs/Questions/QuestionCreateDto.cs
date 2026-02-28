using QuizCore.Domain.Enums;
using System.Collections.Generic;

namespace QuizCore.Application.DTOs.Questions;

public class QuestionCreateDto
{
    public int SubjectId { get; set; }
    public string Topic { get; set; } = string.Empty;
    public QuestionDifficulty Difficulty { get; set; }
    public QuestionType Type { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<AnswerCreateDto> Answers { get; set; } = new();
}

public class AnswerCreateDto
{
    public string Content { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}
