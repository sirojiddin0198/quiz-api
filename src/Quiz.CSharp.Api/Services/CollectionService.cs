namespace Quiz.CSharp.Api.Services;

using AutoMapper;
using Quiz.CSharp.Api.Contracts;
using Quiz.CSharp.Data.Services;
using Quiz.Shared.Authentication;

public sealed class CollectionService(
    ICSharpRepository repository,
    IMapper mapper,
    ICurrentUser currentUser) : ICollectionService
{
    public async Task<List<CollectionResponse>> GetCollectionsAsync(CancellationToken cancellationToken = default)
    {
        var collectionsWithCounts = await repository.GetCollectionsWithQuestionCountAsync(cancellationToken);
        var responses = new List<CollectionResponse>();

        foreach (var collectionWithCount in collectionsWithCounts)
        {
            var response = mapper.Map<CollectionResponse>(collectionWithCount.Collection);
            response = response with { TotalQuestions = collectionWithCount.QuestionCount };

            // Add user progress if authenticated
            if (currentUser.IsAuthenticated && currentUser.UserId is not null)
            {
                var userProgress = await repository.GetUserProgressAsync(
                    currentUser.UserId,
                    collectionWithCount.Collection.Id,
                    cancellationToken);

                if (userProgress is not null)
                {
                    response = response with
                    {
                        UserProgress = mapper.Map<UserProgressResponse>(userProgress)
                    };
                }
            }

            responses.Add(response);
        }

        return responses;
    }

    public async Task<CollectionResponse?> GetCollectionByIdAsync(int collectionId, CancellationToken cancellationToken = default)
    {
        var collection = await repository.GetCollectionByIdAsync(collectionId, cancellationToken);
        if (collection is null) return null;

        var response = mapper.Map<CollectionResponse>(collection);
        
        // Get question count for this collection
        var questionCount = await repository.GetQuestionsByCollectionAsync(collectionId, 1, 1, cancellationToken);
        response = response with { TotalQuestions = questionCount.TotalCount };

        // Add user progress if authenticated
        if (currentUser.IsAuthenticated && currentUser.UserId is not null)
        {
            var userProgress = await repository.GetUserProgressAsync(
                currentUser.UserId,
                collectionId,
                cancellationToken);

            if (userProgress is not null)
            {
                response = response with
                {
                    UserProgress = mapper.Map<UserProgressResponse>(userProgress)
                };
            }
        }

        return response;
    }

    public async Task<CollectionResponse?> GetCollectionByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var collection = await repository.GetCollectionByCodeAsync(code, cancellationToken);
        if (collection is null) return null;

        var response = mapper.Map<CollectionResponse>(collection);
        
        // Get question count for this collection
        var questionCount = await repository.GetQuestionsByCollectionAsync(collection.Id, 1, 1, cancellationToken);
        response = response with { TotalQuestions = questionCount.TotalCount };

        // Add user progress if authenticated
        if (currentUser.IsAuthenticated && currentUser.UserId is not null)
        {
            var userProgress = await repository.GetUserProgressAsync(
                currentUser.UserId,
                collection.Id,
                cancellationToken);

            if (userProgress is not null)
            {
                response = response with
                {
                    UserProgress = mapper.Map<UserProgressResponse>(userProgress)
                };
            }
        }

        return response;
    }
} 