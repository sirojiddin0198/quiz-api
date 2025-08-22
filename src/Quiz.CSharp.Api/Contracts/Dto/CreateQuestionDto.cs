namespace Quiz.CSharp.Api.Contracts.Requests;
public record CreateQuestionDto : CreateQuestionRequest
{
     public required int CollectionId { get; set; }
}