namespace Quiz.CSharp.Api.Contracts;

public sealed record UserProgressResponse
{
    public string? Username { get; init; }
    public string? Name { get; init; }
    public string? TelegramUsername { get; init; }
    public int AnsweredQuestions { get; init; }
    public int CorrectAnswers { get; init; }
    public decimal SuccessRate { get; init; }
    public decimal CompletionRate { get; init; }
}