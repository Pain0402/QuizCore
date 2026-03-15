using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizCore.Application.DTOs.Classes;
using QuizCore.Application.Interfaces.Classes;

namespace QuizCore.API.Controllers;

[ApiController]
[Route("api/v1/classes")]
[Authorize(Roles = "Admin,Teacher")]
public class ClassesController : ControllerBase
{
    private readonly IClassService _classService;
    public ClassesController(IClassService classService) => _classService = classService;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var classes = await _classService.GetAllAsync();
        return Ok(new { success = true, data = classes });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cls = await _classService.GetByIdAsync(id);
        return Ok(new { success = true, data = cls });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] ClassCreateDto dto)
    {
        var cls = await _classService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = cls.Id }, new { success = true, data = cls });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] ClassCreateDto dto)
    {
        var cls = await _classService.UpdateAsync(id, dto);
        return Ok(new { success = true, data = cls });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _classService.DeleteAsync(id);
        return Ok(new { success = true, message = "Đã xóa lớp học" });
    }

    [HttpGet("{id:int}/students")]
    public async Task<IActionResult> GetStudents(int id)
    {
        var students = await _classService.GetStudentsAsync(id);
        return Ok(new { success = true, data = students });
    }

    [HttpPost("{id:int}/students")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignStudents(int id, [FromBody] AssignStudentsDto dto)
    {
        await _classService.AssignStudentsAsync(id, dto);
        return Ok(new { success = true, message = $"Đã thêm {dto.UserIds.Count} học sinh vào lớp" });
    }

    [HttpDelete("{classId:int}/students/{userId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveStudent(int classId, int userId)
    {
        await _classService.RemoveStudentAsync(classId, userId);
        return Ok(new { success = true, message = "Đã xóa học sinh khỏi lớp" });
    }
}
