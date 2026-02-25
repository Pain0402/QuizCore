using QuizCore.Application.DTOs.Auth;
using System.Threading.Tasks;

namespace QuizCore.Application.Interfaces.Auth;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
}
