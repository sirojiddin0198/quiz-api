namespace Quiz.CSharp.Api.Services;

using Quiz.CSharp.Data.Entities;
using System.Text.Json;
using Quiz.CSharp.Api.Services.Abstractions;

public sealed class AnswerValidator : IAnswerValidator
{
    private class MCQMetadata
    {
        public List<MCQOptionData> Options { get; set; } = [];
        public List<string> CorrectAnswerIds { get; set; } = [];
    }

    private class MCQOptionData
    {
        public string? Id { get; set; }
        public string? Text { get; set; }
        public bool IsCorrect { get; set; }
    }

    private class TrueFalseMetadata
    {
        public bool CorrectAnswer { get; set; }
    }

    private class FillMetadata
    {
        public string? CorrectAnswer { get; set; }
    }

    private class ErrorSpottingMetadata
    {
        public string? CorrectAnswer { get; set; }
    }

    private class OutputPredictionMetadata
    {
        public string? ExpectedOutput { get; set; }
    }

    private class CodeWritingMetadata
    {
        public string? Solution { get; set; }
    }

    public Task<bool> ValidateAnswerAsync(
        Question question,
        string userAnswer,
        CancellationToken cancellationToken = default)
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
            var metadata = JsonSerializer.Deserialize<MCQMetadata>(question.Metadata);
            if (metadata == null) return false;

            var userAnswers = JsonSerializer.Deserialize<string[]>(userAnswer);
            var correctAnswers = metadata.CorrectAnswerIds.ToArray();

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
        try
        {
            var metadata = JsonSerializer.Deserialize<TrueFalseMetadata>(question.Metadata);
            if (metadata == null) return false;

            if (bool.TryParse(userAnswer, out var parsed))
                return parsed == metadata.CorrectAnswer;

            return false;
        }
        catch
        {
            return false;
        }
    }

    private static bool ValidateFillAnswer(FillQuestion question, string userAnswer)
    {
        try
        {
            var metadata = JsonSerializer.Deserialize<FillMetadata>(question.Metadata);
            if (metadata?.CorrectAnswer == null) return false;

            var normalizeCode = (string code) => code.Replace("```csharp", "").Replace("```", "").Trim();
            return normalizeCode(metadata.CorrectAnswer) == normalizeCode(userAnswer);
        }
        catch
        {
            return false;
        }
    }

    private static bool ValidateErrorSpottingAnswer(ErrorSpottingQuestion question, string userAnswer)
    {
        try
        {
            var metadata = JsonSerializer.Deserialize<ErrorSpottingMetadata>(question.Metadata);
            if (metadata?.CorrectAnswer == null) return false;

            var normalizeCode = (string code) => code.Replace("```csharp", "").Replace("```", "").Trim();
            return normalizeCode(metadata.CorrectAnswer) == normalizeCode(userAnswer);
        }
        catch
        {
            return false;
        }
    }

    private static bool ValidateOutputPredictionAnswer(OutputPredictionQuestion question, string userAnswer)
    {
        try
        {
            var metadata = JsonSerializer.Deserialize<OutputPredictionMetadata>(question.Metadata);
            if (metadata?.ExpectedOutput == null) return false;

            return string.Equals(metadata.ExpectedOutput.Trim(), userAnswer.Trim(), StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private static bool ValidateCodeWritingAnswer(CodeWritingQuestion question, string userAnswer)
        => string.IsNullOrWhiteSpace(userAnswer) is false;
} 