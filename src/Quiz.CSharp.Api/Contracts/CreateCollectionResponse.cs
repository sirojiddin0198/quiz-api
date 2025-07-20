namespace Quiz.CSharp.Api.Contracts;

public sealed record CreateCollectionResponse
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Icon { get; init; } = string.Empty;
    public int SortOrder { get; init; }
    public int QuestionsCreated { get; init; }
    public DateTime CreatedAt { get; init; }
} 