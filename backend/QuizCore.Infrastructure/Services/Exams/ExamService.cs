using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuizCore.Application.DTOs.Exams;
using QuizCore.Application.Interfaces.Exams;
using QuizCore.Domain.Entities;
using QuizCore.Infrastructure.Data;
using System;

namespace QuizCore.Infrastructure.Services.Exams;

public class ExamService : IExamService
{
    private readonly AppDbContext _context;

    public ExamService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Exam>> GetAllAsync()
    {
        return await _context.Exams.Include(e => e.Subject).ToListAsync();
    }

    public async Task<Exam> GetByIdAsync(int id)
    {
        var exam = await _context.Exams.Include(e => e.ExamQuestions).ThenInclude(eq => eq.Question).FirstOrDefaultAsync(e => e.Id == id);
        if (exam == null) throw new Exception("Exam not found");
        return exam;
    }

    public async Task<Exam> CreateAsync(ExamCreateDto dto, int createdById)
    {
        var exam = new Exam
        {
            Title = dto.Title,
            SubjectId = dto.SubjectId,
            Duration = dto.Duration,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            MaxAttempts = dto.MaxAttempts,
            ShuffleQuestions = dto.ShuffleQuestions,
            CreatedById = createdById,
            ExamQuestions = dto.QuestionIds.Select(qId => new ExamQuestion { QuestionId = qId, ScoreWeight = 1.0f }).ToList()
        };

        _context.Exams.Add(exam);
        await _context.SaveChangesAsync();
        return exam;
    }

    public async Task UpdateAsync(int id, ExamCreateDto dto)
    {
        var exam = await GetByIdAsync(id);
        exam.Title = dto.Title;
        exam.SubjectId = dto.SubjectId;
        exam.Duration = dto.Duration;
        exam.StartTime = dto.StartTime;
        exam.EndTime = dto.EndTime;
        exam.MaxAttempts = dto.MaxAttempts;
        exam.ShuffleQuestions = dto.ShuffleQuestions;
        
        _context.ExamQuestions.RemoveRange(exam.ExamQuestions);
        exam.ExamQuestions = dto.QuestionIds.Select(qId => new ExamQuestion { QuestionId = qId, ScoreWeight = 1.0f }).ToList();
        
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var exam = await GetByIdAsync(id);
        exam.IsDeleted = true;
        await _context.SaveChangesAsync();
    }
}
