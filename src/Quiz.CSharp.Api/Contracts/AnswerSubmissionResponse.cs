namespace Quiz.CSharp.Api.Contracts;

public sealed record AnswerSubmissionResponse
{
    public bool Success { get; init; }
    public bool IsCorrect { get; init; }
    public string? Message { get; init; }
} 