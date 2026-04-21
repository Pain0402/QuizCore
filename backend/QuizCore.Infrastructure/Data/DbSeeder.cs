using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuizCore.Domain.Entities;
using QuizCore.Domain.Enums;
using BCrypt.Net;

namespace QuizCore.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAdminAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        if (!context.Users.Any(u => u.Role == UserRole.Admin))
        {
            var adminUser = new User
            {
                Username = "admin",
                Email = "admin@quizcore.local",
                FullName = "System Administrator",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = UserRole.Admin,
                Status = UserStatus.Active
            };

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();
        }

        if (!context.Users.Any(u => u.Username == "teacher"))
        {
            var teacherUser = new User
            {
                Username = "teacher",
                Email = "teacher@quizcore.local",
                FullName = "Demo Teacher",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher@123"),
                Role = UserRole.Teacher,
                Status = UserStatus.Active
            };
            context.Users.Add(teacherUser);
            await context.SaveChangesAsync();
        }
        
        if (!context.Users.Any(u => u.Username == "student"))
        {
            var studentUser = new User
            {
                Username = "student",
                Email = "student@quizcore.local",
                FullName = "Demo Student",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student@123"),
                Role = UserRole.Student,
                Status = UserStatus.Active
            };
            context.Users.Add(studentUser);
            await context.SaveChangesAsync();
        }
    }
}
