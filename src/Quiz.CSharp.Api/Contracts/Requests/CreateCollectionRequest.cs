namespace Quiz.CSharp.Api.Contracts.Requests;

public sealed record CreateCollectionRequest
{
    public required string Code { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Icon { get; init; }
    public int SortOrder { get; init; }
    public List<CreateQuestionRequest> Questions { get; init; } = [];
}

public sealed record CreateQuestionRequest
{
    public required string Type { get; init; } // mcq, true_false, fill, error_spotting, output_prediction, code_writing
    public required string Subcategory { get; init; }
    public required string Difficulty { get; init; } // Beginner, Intermediate, Advanced
    public required string Prompt { get; init; }
    public int EstimatedTimeMinutes { get; init; }
    public required string Metadata { get; init; } // JSON string containing type-specific data
} 