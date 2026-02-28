using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizCore.Application.DTOs.Questions;
using QuizCore.Application.Interfaces.Questions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QuizCore.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "Admin,Teacher")]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionService _questionService;

    public QuestionsController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] int? subjectId, [FromQuery] string? difficulty)
    {
        var questions = await _questionService.GetAllAsync(subjectId, difficulty);
        return Ok(new { success = true, data = questions });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var question = await _questionService.GetByIdAsync(id);
        return Ok(new { success = true, data = question });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] QuestionCreateDto dto)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out var userId)) return Unauthorized();

        var question = await _questionService.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(Get), new { id = question.Id }, new { success = true, data = question });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] QuestionCreateDto dto)
    {
        await _questionService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _questionService.DeleteAsync(id);
        return NoContent();
    }
}
