namespace Quiz.CSharp.Api.Contracts.Requests;

public sealed record SubmitAnswerRequest
{
    public required int QuestionId { get; init; }
    public required string Answer { get; init; }
    public required int TimeSpentSeconds { get; init; }
} 