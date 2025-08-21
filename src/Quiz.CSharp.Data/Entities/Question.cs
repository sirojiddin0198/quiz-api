namespace Quiz.CSharp.Data.Entities;

using Quiz.Shared.Common;
using System.Text.Json;

public abstract class Question : BaseEntity
{
    public int Id { get; init; }
    public required int CollectionId { get; set; }
    public required string Subcategory { get; init; }
    public required string Difficulty { get; init; }
    public required string Prompt { get; init; }
    public int EstimatedTimeMinutes { get; init; }
    
    public required string Metadata { get; init; }
    
    public Collection Collection { get; init; } = null!;
    public ICollection<UserAnswer> UserAnswers { get; init; } = [];
    
    public T? GetMetadata<T>() where T : class
    {
        try
        {
            return JsonSerializer.Deserialize<T>(Metadata);
        }
        catch
        {
            return null;
        }
    }
}

public sealed class MCQQuestion : Question
{
}

public sealed class TrueFalseQuestion : Question
{
}

public sealed class FillQuestion : Question
{
}

public sealed class ErrorSpottingQuestion : Question
{
}

public sealed class OutputPredictionQuestion : Question
{
}

public sealed class CodeWritingQuestion : Question
{
} 