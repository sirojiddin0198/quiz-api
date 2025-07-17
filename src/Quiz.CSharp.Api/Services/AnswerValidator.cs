namespace Quiz.CSharp.Api.Services;

using Quiz.CSharp.Data.Entities;
using Quiz.CSharp.Data.ValueObjects;
using System.Text.Json;

public sealed class AnswerValidator : IAnswerValidator
{
    public Task<bool> ValidateAnswerAsync(Question question, string userAnswer, CancellationToken cancellationToken = default)
    {
        var result = question switch
        {
            MCQQuestion mcq => ValidateMCQAnswer(mcq, userAnswer),
            TrueFalseQuestion tf => ValidateTrueFalseAnswer(tf, userAnswer),
            FillQuestion fill => ValidateFillAnswer(fill, userAnswer),
            ErrorSpottingQuestion err => ValidateErrorSpottingAnswer(err, userAnswer),
            OutputPredictionQuestion op => ValidateOutputPredictionAnswer(op, userAnswer),
            CodeWritingQuestion code => ValidateCodeWritingAnswer(code, userAnswer),
            _ => false
        };
        return Task.FromResult(result);
    }

    private static bool ValidateMCQAnswer(MCQQuestion question, string userAnswer)
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

    private static bool ValidateTrueFalseAnswer(TrueFalseQuestion question, string userAnswer)
    {
        if (bool.TryParse(userAnswer, out var parsed))
        {
            return parsed == question.CorrectAnswer;
        }
        return false;
    }

    private static bool ValidateFillAnswer(FillQuestion question, string userAnswer)
    {
        var normalizeCode = (string code) => code.Replace("```csharp", "").Replace("```", "").Trim();
        return normalizeCode(question.CorrectAnswer) == normalizeCode(userAnswer);
    }

    private static bool ValidateErrorSpottingAnswer(ErrorSpottingQuestion question, string userAnswer)
    {
        var normalizeCode = (string code) => code.Replace("```csharp", "").Replace("```", "").Trim();
        return normalizeCode(question.CorrectAnswer) == normalizeCode(userAnswer);
    }

    private static bool ValidateOutputPredictionAnswer(OutputPredictionQuestion question, string userAnswer)
    {
        return string.Equals(question.ExpectedOutput.Trim(), userAnswer.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private static bool ValidateCodeWritingAnswer(CodeWritingQuestion question, string userAnswer)
    {
        return !string.IsNullOrWhiteSpace(userAnswer);
    }
} 