namespace Quiz.CSharp.Api.Contracts.Reviews;

public sealed record CompleteSessionRequest
{
    public required List<SessionAnswer> Answers { get; init; }
}

public sealed record SessionAnswer
{
    public required int QuestionId { get; init; }
    public required string Answer { get; init; }
    public required int TimeSpentSeconds { get; init; }
}

public sealed record SessionResultsResponse
{
    public required string SessionId { get; init; }
    public required int TotalQuestions { get; init; }
    public required int CorrectAnswers { get; init; }
    public required decimal ScorePercentage { get; init; }
    public required List<QuestionReviewResponse> ReviewItems { get; init; }
} 