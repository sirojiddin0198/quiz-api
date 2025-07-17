namespace Quiz.CSharp.Data.Entities;

using Quiz.Shared.Common;

public sealed class QuestionHint : BaseEntity
{
    public required int Id { get; init; }
    public required int QuestionId { get; init; }
    public required string Hint { get; init; }
    public int OrderIndex { get; init; }
    public Question Question { get; init; } = null!;
} 