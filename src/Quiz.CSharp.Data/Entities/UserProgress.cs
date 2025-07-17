namespace Quiz.CSharp.Data.Entities;

using Quiz.Shared.Common;

public sealed class UserProgress : BaseEntity
{
    public required string UserId { get; init; }
    public required string CategoryId { get; init; }
    public int TotalQuestions { get; init; }
    public int AnsweredQuestions { get; init; }
    public int CorrectAnswers { get; init; }
    public decimal SuccessRate { get; init; }
    public DateTime LastAnsweredAt { get; init; }
    
    public Category Category { get; init; } = null!;
} 