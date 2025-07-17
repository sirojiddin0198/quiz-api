namespace Quiz.CSharp.Api.Contracts;

public sealed record UserAnswerResponse
{
    public required string Answer { get; init; }
    public required DateTime SubmittedAt { get; init; }
    public required bool IsCorrect { get; init; }
} 