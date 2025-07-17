namespace Quiz.CSharp.Data.Entities;

using Quiz.Shared.Common;

public sealed class MCQOption : BaseEntity
{
    public required string Id { get; init; }
    public required int QuestionId { get; init; }
    public required string Option { get; init; }
    public required bool IsCorrect { get; init; }
    
    public Question Question { get; init; } = null!;
} 