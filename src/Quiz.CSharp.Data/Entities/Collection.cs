namespace Quiz.CSharp.Data.Entities;

using Quiz.Shared.Common;

public sealed class Collection : BaseEntity
{
    public int Id { get; init; }
    public required string Code { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Icon { get; init; }
    public int SortOrder { get; init; }
    public ICollection<Question> Questions { get; init; } = [];
    public ICollection<UserProgress> UserProgress { get; init; } = [];
} 