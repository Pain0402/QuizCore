using QuizCore.Application.DTOs.Classes;

namespace QuizCore.Application.Interfaces.Classes;

public interface IClassService
{
    Task<IEnumerable<ClassDto>> GetAllAsync();
    Task<ClassDto> GetByIdAsync(int id);
    Task<ClassDto> CreateAsync(ClassCreateDto dto);
    Task<ClassDto> UpdateAsync(int id, ClassCreateDto dto);
    Task DeleteAsync(int id);
    Task<IEnumerable<ClassStudentDto>> GetStudentsAsync(int classId);
    Task AssignStudentsAsync(int classId, AssignStudentsDto dto);
    Task RemoveStudentAsync(int classId, int userId);
}
