using System.Collections.Generic;
using System.Threading.Tasks;
using QuizCore.Application.DTOs.Questions;
using QuizCore.Domain.Entities;

namespace QuizCore.Application.Interfaces.Questions;

public interface IQuestionService
{
    Task<IEnumerable<Question>> GetAllAsync(int? subjectId, string? difficulty);
    Task<Question> GetByIdAsync(int id);
    Task<Question> CreateAsync(QuestionCreateDto dto, int createdById);
    Task UpdateAsync(int id, QuestionCreateDto dto);
    Task DeleteAsync(int id);
}
