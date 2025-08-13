namespace Quiz.CSharp.Api.Services.Abstractions;

using Quiz.CSharp.Data.Entities;

public interface IAnswerValidator
{
    Task<bool> ValidateAnswerAsync(
        Question question,
        string userAnswer,
        CancellationToken cancellationToken = default);
} 