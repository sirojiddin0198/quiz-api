namespace Quiz.CSharp.Data.Models;

public record CreateQuestionModel
{
    public required string Type { get; set; }
    public int CollectionId { get; set; }
    public required string Subcategory { get; set; }
    public required string Difficulty { get; set; }
    public required string Prompt { get; set; }
    public int EstimatedTimeMinutes { get; set; }
    public required string Metadata { get; set; }
}