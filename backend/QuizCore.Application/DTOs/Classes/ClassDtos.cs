namespace QuizCore.Application.DTOs.Classes;

public class ClassDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public int StudentCount { get; set; }
}

public class ClassCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
}

public class AssignStudentsDto
{
    public List<int> UserIds { get; set; } = new();
}

public class ClassStudentDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
