using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizCore.Application.DTOs.Exams;
using QuizCore.Application.Interfaces.Exams;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QuizCore.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "Admin,Teacher")]
public class ExamsController : ControllerBase
{
    private readonly IExamService _examService;

    public ExamsController(IExamService examService)
    {
        _examService = examService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var exams = await _examService.GetAllAsync();
        return Ok(new { success = true, data = exams });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var exam = await _examService.GetByIdAsync(id);
        return Ok(new { success = true, data = exam });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ExamCreateDto dto)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out var userId)) return Unauthorized();

        var exam = await _examService.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(Get), new { id = exam.Id }, new { success = true, data = exam });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ExamCreateDto dto)
    {
        await _examService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _examService.DeleteAsync(id);
        return NoContent();
    }
}
