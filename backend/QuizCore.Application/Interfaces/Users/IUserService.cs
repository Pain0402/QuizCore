using QuizCore.Application.DTOs.Users;

namespace QuizCore.Application.Interfaces.Users;

public interface IUserService
{
    Task<IEnumerable<UserListDto>> GetAllAsync(string? role, string? search);
    Task<UserListDto> GetByIdAsync(int id);
    Task<UserListDto> CreateAsync(UserCreateDto dto);
    Task<UserListDto> UpdateAsync(int id, UserUpdateDto dto);
    Task ToggleStatusAsync(int id);
    Task DeleteAsync(int id);
}
