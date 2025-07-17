namespace Quiz.CSharp.Api.Contracts;

public sealed record UserProgressResponse
{
    public int AnsweredQuestions { get; init; }
    public int CorrectAnswers { get; init; }
    public decimal SuccessRate { get; init; }
    public decimal CompletionRate { get; init; }
} 