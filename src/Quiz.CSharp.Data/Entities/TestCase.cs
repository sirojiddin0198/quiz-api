namespace Quiz.CSharp.Data.Entities;

using Quiz.Shared.Common;

public sealed class TestCase : BaseEntity
{
    public required int Id { get; init; }
    public required int QuestionId { get; init; }
    public required string Input { get; init; }
    public required string ExpectedOutput { get; init; }
    
    public Question Question { get; init; } = null!;
} 