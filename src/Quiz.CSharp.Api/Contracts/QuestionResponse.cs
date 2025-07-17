namespace Quiz.CSharp.Api.Contracts;

public sealed record QuestionResponse
{
    public required int Id { get; init; }
    public required string Type { get; init; }
    public required QuestionMetadata Metadata { get; init; }
    public required QuestionContent Content { get; init; }
    public IReadOnlyList<MCQOptionResponse>? Options { get; init; }
    public IReadOnlyList<string>? Hints { get; init; }
    public string? Explanation { get; init; }
    public PreviousAnswerResponse? PreviousAnswer { get; init; }
}

public sealed record QuestionMetadata
{
    public required int CollectionId { get; init; }
    public required string CollectionCode { get; init; }
    public required string Subcategory { get; init; }
    public required string Difficulty { get; init; }
    public int EstimatedTime { get; init; }
}

public sealed record QuestionContent
{
    public required string Prompt { get; init; }
    public string? CodeBefore { get; init; }
    public string? CodeAfter { get; init; }
    public string? CodeWithBlank { get; init; }
    public string? CodeWithError { get; init; }
    public string? Snippet { get; init; }
    public IReadOnlyList<string>? Examples { get; init; }
    public IReadOnlyList<TestCaseResponse>? TestCases { get; init; }
}

public sealed record MCQOptionResponse
{
    public required string Id { get; init; }
    public required string Option { get; init; }
}

public sealed record TestCaseResponse
{
    public required string Input { get; init; }
    public required string ExpectedOutput { get; init; }
}

public sealed record PreviousAnswerResponse
{
    public required string Answer { get; init; }
    public required DateTime SubmittedAt { get; init; }
    public required bool IsCorrect { get; init; }
} 