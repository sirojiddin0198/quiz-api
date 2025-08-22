namespace Quiz.CSharp.Api.Contracts;

public record CreateQuestionResponse
{
    public required int Id { get; set; }
    public required int CollectionId { get; set; }
    public required string Type { get; set; }
    public DateTime CreatedAt { get; set; } 
}