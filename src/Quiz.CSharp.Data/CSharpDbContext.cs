namespace Quiz.CSharp.Data;

using Microsoft.EntityFrameworkCore;
using Quiz.CSharp.Data.Entities;
using System.Reflection;

public sealed class CSharpDbContext(DbContextOptions<CSharpDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<UserAnswer> UserAnswers => Set<UserAnswer>();
    public DbSet<UserProgress> UserProgress => Set<UserProgress>();
    public DbSet<MCQOption> MCQOptions => Set<MCQOption>();
    public DbSet<QuestionHint> QuestionHints => Set<QuestionHint>();
    public DbSet<TestCase> TestCases => Set<TestCase>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("csharp");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
} 