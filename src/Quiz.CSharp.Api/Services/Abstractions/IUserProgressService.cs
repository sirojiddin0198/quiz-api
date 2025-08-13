namespace Quiz.CSharp.Api.Services.Abstractions;

using Quiz.CSharp.Api.Contracts;

public interface IUserProgressService
{
    Task<List<CollectionProgressResponse>> GetUserProgressAsync(CancellationToken cancellationToken = default);
} 