using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuizCore.Application.DTOs.Auth;
using QuizCore.Application.Interfaces.Auth;

namespace QuizCore.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        if (response == null)
            return Unauthorized(new { success = false, message = "Invalid username or password" });

        return Ok(new { success = true, data = response });
    }
}
