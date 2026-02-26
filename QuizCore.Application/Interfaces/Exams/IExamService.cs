using System.Collections.Generic;
using System.Threading.Tasks;
using QuizCore.Application.DTOs.Exams;
using QuizCore.Domain.Entities;

namespace QuizCore.Application.Interfaces.Exams;

public interface IExamService
{
    Task<IEnumerable<Exam>> GetAllAsync();
    Task<Exam> GetByIdAsync(int id);
    Task<Exam> CreateAsync(ExamCreateDto dto, int createdById);
    Task UpdateAsync(int id, ExamCreateDto dto);
    Task DeleteAsync(int id);
}
