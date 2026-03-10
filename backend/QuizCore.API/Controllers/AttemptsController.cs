using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizCore.Application.DTOs.Attempts;
using QuizCore.Application.Interfaces.Attempts;

namespace QuizCore.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class AttemptsController(IAttemptService attemptService) : ControllerBase
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Start a new exam attempt</summary>
    [HttpPost("exams/{examId}/start")]
    public async Task<IActionResult> Start(int examId)
    {
        var result = await attemptService.StartExamAsync(examId, UserId);
        return Ok(new { success = true, data = result });
    }

    /// <summary>Autosave one question answer to Redis</summary>
    [HttpPut("{attemptId}/autosave")]
    public async Task<IActionResult> AutoSave(int attemptId, [FromBody] AutoSaveRequest request)
    {
        await attemptService.AutoSaveAsync(attemptId, UserId, request);
        return Ok(new { success = true, message = "Đã lưu câu trả lời" });
    }

    /// <summary>Get current progress (from Redis) to restore on reconnect</summary>
    [HttpGet("{attemptId}/progress")]
    public async Task<IActionResult> GetProgress(int attemptId)
    {
        var result = await attemptService.GetProgressAsync(attemptId, UserId);
        return Ok(new { success = true, data = result });
    }

    /// <summary>Submit exam and get result immediately</summary>
    [HttpPost("{attemptId}/submit")]
    public async Task<IActionResult> Submit(int attemptId)
    {
        var result = await attemptService.SubmitAsync(attemptId, UserId);
        return Ok(new { success = true, data = result });
    }

    /// <summary>Get result of a completed attempt</summary>
    [HttpGet("{attemptId}/result")]
    public async Task<IActionResult> GetResult(int attemptId)
    {
        var result = await attemptService.GetResultAsync(attemptId, UserId);
        return Ok(new { success = true, data = result });
    }
}
