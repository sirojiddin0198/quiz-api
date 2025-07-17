namespace Quiz.CSharp.Data.Entities;

using Quiz.Shared.Common;

public sealed class Category : BaseEntity
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Icon { get; init; }
    public int SortOrder { get; init; }
    
    public ICollection<Question> Questions { get; init; } = [];
    public ICollection<UserProgress> UserProgress { get; init; } = [];
} 