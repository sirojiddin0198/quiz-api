namespace Quiz.CSharp.Api.Contracts;

public sealed record CategoryResponse
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Icon { get; init; }
    public int TotalQuestions { get; init; }
    public UserProgressResponse? UserProgress { get; init; }
} 