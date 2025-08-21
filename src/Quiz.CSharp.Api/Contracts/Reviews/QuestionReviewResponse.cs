namespace Quiz.CSharp.Api.Contracts.Reviews;

public sealed record QuestionReviewResponse
{
    public required int QuestionId { get; init; }
    public required string QuestionType { get; init; }
    public required string Prompt { get; init; }
    public required QuestionContentReview Content { get; init; }
    public required UserAnswerReview? UserAnswer { get; init; }
    public required CorrectAnswerReview CorrectAnswer { get; init; }
    public required string? Explanation { get; init; }
    public required List<string>? Hints { get; init; }
}

public sealed record QuestionContentReview
{
    public string? CodeBefore { get; init; }
    public string? CodeAfter { get; init; }
    public string? CodeWithBlank { get; init; }
    public string? CodeWithError { get; init; }
    public string? Snippet { get; init; }
}

public sealed record UserAnswerReview
{
    public required string Answer { get; init; }
    public required bool IsCorrect { get; init; }
    public required DateTime SubmittedAt { get; init; }
    public required int TimeSpentSeconds { get; init; }
}

public sealed record CorrectAnswerReview
{
    public List<MCQCorrectOption>? Options { get; init; }
    
    public bool? BooleanAnswer { get; init; }
    
    public string? TextAnswer { get; init; }
    
    public string? SampleSolution { get; init; }
    public List<TestCaseResult>? TestCaseResults { get; init; }
}

public sealed record MCQCorrectOption
{
    public required string Id { get; init; }
    public required string Text { get; init; }
    public required bool IsCorrect { get; init; }
}

public sealed record TestCaseResult
{
    public required string Input { get; init; }
    public required string ExpectedOutput { get; init; }
    public string? UserOutput { get; init; }
    public required bool Passed { get; init; }
} 