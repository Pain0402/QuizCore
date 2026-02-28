using System.Collections.Generic;

namespace QuizCore.Domain.Entities;

public class Class
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? AcademicYear { get; set; }

    public ICollection<UserClass> UserClasses { get; set; } = new List<UserClass>();
}
