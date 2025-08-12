namespace Quiz.CSharp.Api.Dtos;
public sealed record CreateCollectionDto
{
    public required string Code { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Icon { get; init; }
    public int SortOrder { get; init; }
   
}