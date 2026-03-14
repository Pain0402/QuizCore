using Microsoft.EntityFrameworkCore;
using QuizCore.Application.DTOs.Classes;
using QuizCore.Application.Interfaces.Classes;
using QuizCore.Domain.Entities;
using QuizCore.Infrastructure.Data;

namespace QuizCore.Infrastructure.Services.Classes;

public class ClassService : IClassService
{
    private readonly AppDbContext _context;

    public ClassService(AppDbContext context) => _context = context;

    public async Task<IEnumerable<ClassDto>> GetAllAsync()
    {
        return await _context.Classes
            .Select(c => new ClassDto
            {
                Id = c.Id,
                Name = c.Name,
                AcademicYear = c.AcademicYear,
                StudentCount = c.UserClasses.Count
            })
            .OrderBy(c => c.AcademicYear).ThenBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<ClassDto> GetByIdAsync(int id)
    {
        var cls = await _context.Classes.Include(c => c.UserClasses)
            .FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new Exception("Không tìm thấy lớp học");

        return new ClassDto
        {
            Id = cls.Id,
            Name = cls.Name,
            AcademicYear = cls.AcademicYear,
            StudentCount = cls.UserClasses.Count
        };
    }

    public async Task<ClassDto> CreateAsync(ClassCreateDto dto)
    {
        if (await _context.Classes.AnyAsync(c => c.Name == dto.Name && c.AcademicYear == dto.AcademicYear))
            throw new Exception($"Lớp '{dto.Name}' năm học '{dto.AcademicYear}' đã tồn tại");

        var cls = new Class { Name = dto.Name, AcademicYear = dto.AcademicYear };
        _context.Classes.Add(cls);
        await _context.SaveChangesAsync();
        return new ClassDto { Id = cls.Id, Name = cls.Name, AcademicYear = cls.AcademicYear, StudentCount = 0 };
    }

    public async Task<ClassDto> UpdateAsync(int id, ClassCreateDto dto)
    {
        var cls = await _context.Classes.FindAsync(id)
            ?? throw new Exception("Không tìm thấy lớp học");

        cls.Name = dto.Name;
        cls.AcademicYear = dto.AcademicYear;
        await _context.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var cls = await _context.Classes.Include(c => c.UserClasses)
            .FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new Exception("Không tìm thấy lớp học");

        if (cls.UserClasses.Any())
            throw new Exception("Không thể xóa lớp đang có học sinh");

        _context.Classes.Remove(cls);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ClassStudentDto>> GetStudentsAsync(int classId)
    {
        return await _context.UserClasses
            .Where(uc => uc.ClassId == classId)
            .Select(uc => new ClassStudentDto
            {
                UserId = uc.UserId,
                Username = uc.User.Username,
                FullName = uc.User.FullName,
                Email = uc.User.Email
            })
            .ToListAsync();
    }

    public async Task AssignStudentsAsync(int classId, AssignStudentsDto dto)
    {
        var cls = await _context.Classes.FindAsync(classId)
            ?? throw new Exception("Không tìm thấy lớp học");

        var existing = await _context.UserClasses
            .Where(uc => uc.ClassId == classId)
            .Select(uc => uc.UserId)
            .ToListAsync();

        var toAdd = dto.UserIds.Except(existing).ToList();
        foreach (var userId in toAdd)
        {
            _context.UserClasses.Add(new UserClass { UserId = userId, ClassId = classId });
        }

        await _context.SaveChangesAsync();
    }

    public async Task RemoveStudentAsync(int classId, int userId)
    {
        var uc = await _context.UserClasses
            .FirstOrDefaultAsync(x => x.ClassId == classId && x.UserId == userId)
            ?? throw new Exception("Học sinh không thuộc lớp này");

        _context.UserClasses.Remove(uc);
        await _context.SaveChangesAsync();
    }
}
