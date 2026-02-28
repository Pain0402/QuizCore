using System;
using System.Collections.Generic;

namespace QuizCore.Application.DTOs.Exams;

public class ExamCreateDto
{
    public string Title { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public int Duration { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int MaxAttempts { get; set; } = 1;
    public bool ShuffleQuestions { get; set; } = true;
    public List<int> QuestionIds { get; set; } = new();
}
