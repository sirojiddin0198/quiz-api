namespace Quiz.CSharp.Api.Contracts;

public sealed record CollectionResponse
{
    public required int Id { get; init; }
    public required string Code { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Icon { get; init; }
    public int SortOrder { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public UserProgressResponse? UserProgress { get; init; }
} 