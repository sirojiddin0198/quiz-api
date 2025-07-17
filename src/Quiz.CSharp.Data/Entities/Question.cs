namespace Quiz.CSharp.Data.Entities;

using Quiz.Shared.Common;

public abstract class Question : BaseEntity
{
    public int Id { get; init; }
    public required int CollectionId { get; init; }
    public required string Subcategory { get; init; }
    public required string Difficulty { get; init; }
    public required string Prompt { get; init; }
    public int EstimatedTimeMinutes { get; init; }
    public Collection Collection { get; init; } = null!;
    public ICollection<UserAnswer> UserAnswers { get; init; } = [];
    public ICollection<QuestionHint> Hints { get; init; } = [];
}

public sealed class MCQQuestion : Question
{
    public ICollection<MCQOption> Options { get; init; } = [];
}

public sealed class TrueFalseQuestion : Question
{
    public bool CorrectAnswer { get; init; }
}

public sealed class FillQuestion : Question
{
    public string CorrectAnswer { get; init; } = string.Empty;
    public ICollection<string> FillHints { get; init; } = [];
}

public sealed class ErrorSpottingQuestion : Question
{
    public string CorrectAnswer { get; init; } = string.Empty;
}

public sealed class OutputPredictionQuestion : Question
{
    public string ExpectedOutput { get; init; } = string.Empty;
}

public sealed class CodeWritingQuestion : Question
{
    public string? Solution { get; init; }
    public ICollection<string> Examples { get; init; } = [];
    public ICollection<TestCase> TestCases { get; init; } = [];
    public ICollection<string> Rubric { get; init; } = [];
} 