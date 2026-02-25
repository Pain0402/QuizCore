using QuizCore.Domain.Entities;

namespace QuizCore.Application.Interfaces.Auth;

public interface ITokenGenerator
{
    string GenerateToken(User user, out int expiresIn);
}
