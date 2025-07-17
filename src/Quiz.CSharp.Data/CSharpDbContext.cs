namespace Quiz.CSharp.Data;

using Microsoft.EntityFrameworkCore;
using Quiz.CSharp.Data.Entities;
using System.Reflection;

public sealed class CSharpDbContext(DbContextOptions<CSharpDbContext> options) : DbContext(options)
{
    public DbSet<Collection> Collections => Set<Collection>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<MCQQuestion> MCQQuestions => Set<MCQQuestion>();
    public DbSet<TrueFalseQuestion> TrueFalseQuestions => Set<TrueFalseQuestion>();
    public DbSet<FillQuestion> FillQuestions => Set<FillQuestion>();
    public DbSet<ErrorSpottingQuestion> ErrorSpottingQuestions => Set<ErrorSpottingQuestion>();
    public DbSet<OutputPredictionQuestion> OutputPredictionQuestions => Set<OutputPredictionQuestion>();
    public DbSet<CodeWritingQuestion> CodeWritingQuestions => Set<CodeWritingQuestion>();
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