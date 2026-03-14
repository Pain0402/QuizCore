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
    }
}
