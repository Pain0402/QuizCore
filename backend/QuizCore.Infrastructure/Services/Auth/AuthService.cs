using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuizCore.Application.DTOs.Auth;
using QuizCore.Application.Interfaces.Auth;
using QuizCore.Infrastructure.Data;

namespace QuizCore.Infrastructure.Services.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly ITokenGenerator _tokenGenerator;

    public AuthService(AppDbContext context, ITokenGenerator tokenGenerator)
    {
        _context = context;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (user == null || user.Status != QuizCore.Domain.Enums.UserStatus.Active) return null;

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        var token = _tokenGenerator.GenerateToken(user, out var expiresIn);

        return new AuthResponse
        {
            Token = token,
            ExpiresIn = expiresIn,
            User = new UserDto { Id = user.Id, FullName = user.FullName, Role = user.Role.ToString() }
        };
    }
}
