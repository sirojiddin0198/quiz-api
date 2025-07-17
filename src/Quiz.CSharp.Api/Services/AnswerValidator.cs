namespace Quiz.CSharp.Api.Services;

using Quiz.CSharp.Data.Entities;
using Quiz.CSharp.Data.ValueObjects;
using System.Text.Json;

public sealed class AnswerValidator : IAnswerValidator
{
    public Task<bool> ValidateAnswerAsync(Question question, string userAnswer, CancellationToken cancellationToken = default)
    {
        var result = question.Type switch
        {
            QuestionType.MCQ => ValidateMCQAnswer(question, userAnswer),
            QuestionType.TrueFalse => ValidateTrueFalseAnswer(question, userAnswer),
            QuestionType.Fill => ValidateFillAnswer(question, userAnswer),
            QuestionType.ErrorSpotting => ValidateErrorSpottingAnswer(question, userAnswer),
            QuestionType.OutputPrediction => ValidateOutputPredictionAnswer(question, userAnswer),
            QuestionType.CodeWriting => ValidateCodeWritingAnswer(question, userAnswer),
            _ => false
        };
        
        return Task.FromResult(result);
    }

    private static bool ValidateMCQAnswer(Question question, string userAnswer)
    {
        try
        {
            var userAnswers = JsonSerializer.Deserialize<string[]>(userAnswer);
            var correctAnswers = question.Options
                .Where(o => o.IsCorrect)
                .Select(o => o.Id)
                .ToArray();

            return userAnswers?.Length == correctAnswers.Length &&
                   userAnswers.All(ua => correctAnswers.Contains(ua));
        }
        catch
        {
            return false;
        }
    }

    private static bool ValidateTrueFalseAnswer(Question question, string userAnswer)
    {
        var correctAnswer = question.Options.FirstOrDefault(o => o.IsCorrect);
        return correctAnswer is not null && 
               string.Equals(userAnswer, correctAnswer.Id, StringComparison.OrdinalIgnoreCase);
    }

    private static bool ValidateFillAnswer(Question question, string userAnswer)
    {
        var normalizeCode = (string code) => code.Replace("```csharp", "").Replace("```", "").Trim();
        
        var correctAnswer = question.Options.FirstOrDefault(o => o.IsCorrect);
        return correctAnswer is not null && 
               normalizeCode(correctAnswer.Option) == normalizeCode(userAnswer);
    }

    private static bool ValidateErrorSpottingAnswer(Question question, string userAnswer)
    {
        var normalizeCode = (string code) => code.Replace("```csharp", "").Replace("```", "").Trim();
        
        var correctAnswer = question.Options.FirstOrDefault(o => o.IsCorrect);
        return correctAnswer is not null && 
               normalizeCode(correctAnswer.Option) == normalizeCode(userAnswer);
    }

    private static bool ValidateOutputPredictionAnswer(Question question, string userAnswer)
    {
        var correctAnswer = question.Options.FirstOrDefault(o => o.IsCorrect);
        return correctAnswer is not null && 
               string.Equals(correctAnswer.Option.Trim(), userAnswer.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private static bool ValidateCodeWritingAnswer(Question question, string userAnswer)
    {
        return !string.IsNullOrWhiteSpace(userAnswer);
    }
} 