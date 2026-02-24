using Microsoft.EntityFrameworkCore;
using QuizCore.Domain.Entities;

namespace QuizCore.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Class> Classes { get; set; } = null!;
    public DbSet<UserClass> UserClasses { get; set; } = null!;
    public DbSet<Subject> Subjects { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<MediaFile> MediaFiles { get; set; } = null!;
    public DbSet<Question> Questions { get; set; } = null!;
    public DbSet<Answer> Answers { get; set; } = null!;
    public DbSet<Exam> Exams { get; set; } = null!;
    public DbSet<ExamQuestion> ExamQuestions { get; set; } = null!;
    public DbSet<ExamAttempt> ExamAttempts { get; set; } = null!;
    public DbSet<AttemptDetail> AttemptDetails { get; set; } = null!;
    public DbSet<ExamLog> ExamLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User setup
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Role).HasConversion<string>();
            entity.Property(e => e.Status).HasConversion<string>();
        });

        // UserClass (Many-to-Many)
        modelBuilder.Entity<UserClass>(entity =>
        {
            entity.HasKey(uc => new { uc.UserId, uc.ClassId });
            
            entity.HasOne(uc => uc.User)
                .WithMany(u => u.UserClasses)
                .HasForeignKey(uc => uc.UserId);
                
            entity.HasOne(uc => uc.Class)
                .WithMany(c => c.UserClasses)
                .HasForeignKey(uc => uc.ClassId);
        });

        // Question
        modelBuilder.Entity<Question>(entity =>
        {
            entity.Property(e => e.Difficulty).HasConversion<string>();
            entity.Property(e => e.Type).HasConversion<string>();
            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasOne(q => q.CreatedBy)
                .WithMany(u => u.CreatedQuestions)
                .HasForeignKey(q => q.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Category
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasOne(c => c.Parent)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Exam
        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasOne(e => e.CreatedBy)
                .WithMany(u => u.CreatedExams)
                .HasForeignKey(e => e.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ExamQuestion (Many-to-Many)
        modelBuilder.Entity<ExamQuestion>(entity =>
        {
            entity.HasKey(eq => new { eq.ExamId, eq.QuestionId });
            
            entity.HasOne(eq => eq.Exam)
                .WithMany(e => e.ExamQuestions)
                .HasForeignKey(eq => eq.ExamId);
                
            entity.HasOne(eq => eq.Question)
                .WithMany(q => q.ExamQuestions)
                .HasForeignKey(eq => eq.QuestionId);
        });

        // ExamAttempt
        modelBuilder.Entity<ExamAttempt>(entity =>
        {
            entity.Property(e => e.Status).HasConversion<string>();
            entity.HasQueryFilter(e => !e.IsDeleted);
        });
        
        // AttemptDetail and ExamLog are implicitly mapped via foreign keys but we can be explicit if needed
    }
}
