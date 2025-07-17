namespace Quiz.CSharp.Data.Entities;

using Quiz.Shared.Common;
using Quiz.CSharp.Data.ValueObjects;

public sealed class Question : BaseEntity
{
    public required int Id { get; init; }
    public required QuestionType Type { get; init; }
    public required string CategoryId { get; init; }
    public required string Subcategory { get; init; }
    public required string Difficulty { get; init; }
    public required string Prompt { get; init; }
    public string? CodeBefore { get; init; }
    public string? CodeAfter { get; init; }
    public string? CodeWithBlank { get; init; }
    public string? CodeWithError { get; init; }
    public string? Snippet { get; init; }
    public string? Explanation { get; init; }
    public int EstimatedTimeMinutes { get; init; }
    
    public Category Category { get; init; } = null!;
    public ICollection<MCQOption> Options { get; init; } = [];
    public ICollection<UserAnswer> UserAnswers { get; init; } = [];
    public ICollection<QuestionHint> Hints { get; init; } = [];
    public ICollection<TestCase> TestCases { get; init; } = [];
} 