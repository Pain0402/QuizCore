using Microsoft.EntityFrameworkCore;
using QuizCore.Application.DTOs.Users;
using QuizCore.Application.Interfaces.Users;
using QuizCore.Domain.Entities;
using QuizCore.Domain.Enums;
using QuizCore.Infrastructure.Data;
using BCrypt.Net;

namespace QuizCore.Infrastructure.Services.Users;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context) => _context = context;

    private static UserListDto ToDto(User u) => new()
    {
        Id = u.Id,
        Username = u.Username,
        FullName = u.FullName,
        Email = u.Email,
        Role = u.Role.ToString(),
        Status = u.Status.ToString(),
        CreatedAt = u.CreatedAt
    };

    public async Task<IEnumerable<UserListDto>> GetAllAsync(string? role, string? search)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(role) && Enum.TryParse<UserRole>(role, true, out var r))
            query = query.Where(u => u.Role == r);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(u => u.FullName.Contains(search) || u.Username.Contains(search) || u.Email.Contains(search));

        return await query.OrderByDescending(u => u.CreatedAt).Select(u => new UserListDto
        {
            Id = u.Id,
            Username = u.Username,
            FullName = u.FullName,
            Email = u.Email,
            Role = u.Role.ToString(),
            Status = u.Status.ToString(),
            CreatedAt = u.CreatedAt
        }).ToListAsync();
    }

    public async Task<UserListDto> GetByIdAsync(int id)
    {
        var user = await _context.Users.FindAsync(id)
            ?? throw new Exception("Không tìm thấy người dùng");
        return ToDto(user);
    }

    public async Task<UserListDto> CreateAsync(UserCreateDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            throw new Exception($"Tên đăng nhập '{dto.Username}' đã tồn tại");

        if (!string.IsNullOrEmpty(dto.Email) && await _context.Users.AnyAsync(u => u.Email == dto.Email))
            throw new Exception($"Email '{dto.Email}' đã được sử dụng");

        if (!Enum.TryParse<UserRole>(dto.Role, true, out var role))
            throw new Exception("Vai trò không hợp lệ");

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            FullName = dto.FullName,
            Email = dto.Email,
            Role = role,
            Status = UserStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return ToDto(user);
    }

    public async Task<UserListDto> UpdateAsync(int id, UserUpdateDto dto)
    {
        var user = await _context.Users.FindAsync(id)
            ?? throw new Exception("Không tìm thấy người dùng");

        if (!string.IsNullOrEmpty(dto.Email) && dto.Email != user.Email
            && await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != id))
            throw new Exception($"Email '{dto.Email}' đã được sử dụng");

        if (!Enum.TryParse<UserRole>(dto.Role, true, out var role))
            throw new Exception("Vai trò không hợp lệ");

        user.FullName = dto.FullName;
        user.Email = dto.Email;
        user.Role = role;

        if (!string.IsNullOrEmpty(dto.NewPassword))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

        await _context.SaveChangesAsync();
        return ToDto(user);
    }

    public async Task ToggleStatusAsync(int id)
    {
        var user = await _context.Users.FindAsync(id)
            ?? throw new Exception("Không tìm thấy người dùng");

        user.Status = user.Status == UserStatus.Active ? UserStatus.Inactive : UserStatus.Active;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id)
            ?? throw new Exception("Không tìm thấy người dùng");
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}
