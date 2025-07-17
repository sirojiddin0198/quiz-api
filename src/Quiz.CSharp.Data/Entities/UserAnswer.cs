namespace Quiz.CSharp.Data.Entities;

using Quiz.Shared.Common;

public sealed class UserAnswer : BaseEntity
{
    public int Id { get; init; }
    public required string UserId { get; init; }
    public required int QuestionId { get; init; }
    public required string Answer { get; init; }
    public required bool IsCorrect { get; init; }
    public required int TimeSpentSeconds { get; init; }
    public DateTime SubmittedAt { get; init; } = DateTime.UtcNow;
    public int AttemptNumber { get; init; }
    public Question Question { get; init; } = null!;
} 