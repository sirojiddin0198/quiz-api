namespace Quiz.CSharp.Api.Contracts.Reviews;

public sealed record CollectionResultsResponse
{
    public required int CollectionId { get; init; }
    public required string CollectionName { get; init; }
    public required int TotalQuestions { get; init; }
    public required int AnsweredQuestions { get; init; }
    public required int CorrectAnswers { get; init; }
    public required decimal ScorePercentage { get; init; }
    public required TimeSpan TotalTimeSpent { get; init; }
    public required DateTime CompletedAt { get; init; }
} 