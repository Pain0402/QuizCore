using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuizCore.Application.DTOs.Questions;
using QuizCore.Application.Interfaces.Questions;
using QuizCore.Domain.Entities;
using QuizCore.Infrastructure.Data;
using QuizCore.Domain.Enums;
using System;

namespace QuizCore.Infrastructure.Services.Questions;

public class QuestionService : IQuestionService
{
    private readonly AppDbContext _context;

    public QuestionService(AppDbContext context)
    {
        _context = context;
    }

    private static QuestionDifficulty ParseDifficulty(string? s)
        => Enum.TryParse<QuestionDifficulty>(s, true, out var d) ? d : QuestionDifficulty.Medium;

    private static QuestionType ParseType(string? s)
        => Enum.TryParse<QuestionType>(s, true, out var t) ? t : QuestionType.SingleChoice;

    public async Task<IEnumerable<Question>> GetAllAsync(int? subjectId, string? difficulty)
    {
        var query = _context.Questions
            .Include(q => q.Answers)
            .AsQueryable();

        if (subjectId.HasValue)
            query = query.Where(q => q.SubjectId == subjectId.Value);

        if (!string.IsNullOrEmpty(difficulty) && Enum.TryParse<QuestionDifficulty>(difficulty, true, out var dif))
            query = query.Where(q => q.Difficulty == dif);

        return await query.ToListAsync();
    }

    public async Task<Question> GetByIdAsync(int id)
    {
        var question = await _context.Questions
            .Include(q => q.Answers)
            .FirstOrDefaultAsync(q => q.Id == id);
        if (question == null) throw new Exception("Question not found");
        return question;
    }

    public async Task<Question> CreateAsync(QuestionCreateDto dto, int createdById)
    {
        var question = new Question
        {
            SubjectId    = dto.SubjectId > 0 ? dto.SubjectId : 1,
            Topic        = dto.Topic,
            Difficulty   = ParseDifficulty(dto.Difficulty),
            QuestionType = ParseType(dto.QuestionType),
            Content      = dto.Content,
            CreatedById  = createdById,
            Answers      = dto.Answers.Select(a => new Answer { Content = a.Content, IsCorrect = a.IsCorrect }).ToList()
        };

        _context.Questions.Add(question);
        await _context.SaveChangesAsync();
        return question;
    }

    public async Task UpdateAsync(int id, QuestionCreateDto dto)
    {
        var question = await GetByIdAsync(id);
        question.SubjectId    = dto.SubjectId > 0 ? dto.SubjectId : question.SubjectId;
        question.Topic        = dto.Topic;
        question.Difficulty   = ParseDifficulty(dto.Difficulty);
        question.QuestionType = ParseType(dto.QuestionType);
        question.Content      = dto.Content;

        _context.Answers.RemoveRange(question.Answers);
        question.Answers = dto.Answers
            .Select(a => new Answer { Content = a.Content, IsCorrect = a.IsCorrect })
            .ToList();

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var question = await GetByIdAsync(id);
        question.IsDeleted = true;
        await _context.SaveChangesAsync();
    }
}
