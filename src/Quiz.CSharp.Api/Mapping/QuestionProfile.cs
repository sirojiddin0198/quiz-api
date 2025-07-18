namespace Quiz.CSharp.Api.Mapping;

using AutoMapper;
using Quiz.CSharp.Api.Contracts;
using Quiz.CSharp.Data.Entities;
using System.Text.Json;

public sealed class QuestionProfile : Profile
{
    // Internal metadata classes for deserialization
    private class QuestionMetadataBase
    {
        public string? CodeBefore { get; set; }
        public string? CodeAfter { get; set; }
        public List<QuestionHintData> Hints { get; set; } = [];
        public string? Explanation { get; set; }
    }

    private class MCQMetadata : QuestionMetadataBase
    {
        public List<MCQOptionData> Options { get; set; } = [];
        public List<string> CorrectAnswerIds { get; set; } = [];
    }

    private class FillMetadata : QuestionMetadataBase
    {
        public string? CodeWithBlank { get; set; }
        public string CorrectAnswer { get; set; } = string.Empty;
    }

    private class ErrorSpottingMetadata : QuestionMetadataBase
    {
        public string? CodeWithError { get; set; }
        public string CorrectAnswer { get; set; } = string.Empty;
    }

    private class OutputPredictionMetadata : QuestionMetadataBase
    {
        public string? Snippet { get; set; }
        public string ExpectedOutput { get; set; } = string.Empty;
    }

    private class CodeWritingMetadata : QuestionMetadataBase
    {
        public string? Solution { get; set; }
        public List<string> Examples { get; set; } = [];
        public List<TestCaseData> TestCases { get; set; } = [];
    }

    private class MCQOptionData
    {
        public string Id { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }

    private class QuestionHintData
    {
        public string Hint { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
    }

    private class TestCaseData
    {
        public string Input { get; set; } = string.Empty;
        public string ExpectedOutput { get; set; } = string.Empty;
    }

    public QuestionProfile()
    {
        CreateMap<Question, QuestionResponse>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => GetQuestionType(src)))
            .ForMember(dest => dest.Metadata, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => GetMCQOptions(src)))
            .ForMember(dest => dest.Hints, opt => opt.MapFrom(src => GetHints(src)))
            .ForMember(dest => dest.Explanation, opt => opt.MapFrom(src => GetExplanation(src)));

        CreateMap<Question, QuestionMetadata>()
            .ForMember(dest => dest.CollectionId, opt => opt.MapFrom(src => src.CollectionId))
            .ForMember(dest => dest.CollectionCode, opt => opt.MapFrom(src => src.Collection.Code))
            .ForMember(dest => dest.EstimatedTime, opt => opt.MapFrom(src => src.EstimatedTimeMinutes));

        CreateMap<Question, QuestionContent>()
            .ForMember(dest => dest.CodeBefore, opt => opt.MapFrom(src => GetCodeBefore(src)))
            .ForMember(dest => dest.CodeAfter, opt => opt.MapFrom(src => GetCodeAfter(src)))
            .ForMember(dest => dest.CodeWithBlank, opt => opt.MapFrom(src => GetCodeWithBlank(src)))
            .ForMember(dest => dest.CodeWithError, opt => opt.MapFrom(src => GetCodeWithError(src)))
            .ForMember(dest => dest.Snippet, opt => opt.MapFrom(src => GetSnippet(src)))
            .ForMember(dest => dest.Examples, opt => opt.MapFrom(src => GetExamples(src)))
            .ForMember(dest => dest.TestCases, opt => opt.MapFrom(src => GetTestCases(src)));

        CreateMap<MCQOptionData, MCQOptionResponse>()
            .ForMember(dest => dest.Option, opt => opt.MapFrom(src => src.Text));
            
        CreateMap<TestCaseData, TestCaseResponse>();
    }

    private static string GetQuestionType(Question question)
    {
        return question switch
        {
            MCQQuestion => "MCQ",
            TrueFalseQuestion => "TrueFalse",
            FillQuestion => "Fill",
            ErrorSpottingQuestion => "ErrorSpotting",
            OutputPredictionQuestion => "OutputPrediction",
            CodeWritingQuestion => "CodeWriting",
            _ => "Unknown"
        };
    }

    private static ICollection<MCQOptionResponse>? GetMCQOptions(Question question)
    {
        if (question is not MCQQuestion) return null;

        try
        {
            var metadata = JsonSerializer.Deserialize<MCQMetadata>(question.Metadata);
            return metadata?.Options.Select(o => new MCQOptionResponse
            {
                Id = o.Id,
                Option = o.Text
            }).ToList();
        }
        catch
        {
            return null;
        }
    }

    private static ICollection<string>? GetHints(Question question)
    {
        try
        {
            var metadata = JsonSerializer.Deserialize<QuestionMetadataBase>(question.Metadata);
            return metadata?.Hints
                .OrderBy(h => h.OrderIndex)
                .Select(h => h.Hint)
                .ToList();
        }
        catch
        {
            return null;
        }
    }

    private static string? GetExplanation(Question question)
    {
        try
        {
            var metadata = JsonSerializer.Deserialize<QuestionMetadataBase>(question.Metadata);
            return metadata?.Explanation;
        }
        catch
        {
            return null;
        }
    }

    private static string? GetCodeBefore(Question question)
    {
        try
        {
            var metadata = JsonSerializer.Deserialize<QuestionMetadataBase>(question.Metadata);
            return metadata?.CodeBefore;
        }
        catch
        {
            return null;
        }
    }

    private static string? GetCodeAfter(Question question)
    {
        try
        {
            var metadata = JsonSerializer.Deserialize<QuestionMetadataBase>(question.Metadata);
            return metadata?.CodeAfter;
        }
        catch
        {
            return null;
        }
    }

    private static string? GetCodeWithBlank(Question question)
    {
        if (question is not FillQuestion) return null;

        try
        {
            var metadata = JsonSerializer.Deserialize<FillMetadata>(question.Metadata);
            return metadata?.CodeWithBlank;
        }
        catch
        {
            return null;
        }
    }

    private static string? GetCodeWithError(Question question)
    {
        if (question is not ErrorSpottingQuestion) return null;

        try
        {
            var metadata = JsonSerializer.Deserialize<ErrorSpottingMetadata>(question.Metadata);
            return metadata?.CodeWithError;
        }
        catch
        {
            return null;
        }
    }

    private static string? GetSnippet(Question question)
    {
        if (question is not OutputPredictionQuestion) return null;

        try
        {
            var metadata = JsonSerializer.Deserialize<OutputPredictionMetadata>(question.Metadata);
            return metadata?.Snippet;
        }
        catch
        {
            return null;
        }
    }

    private static ICollection<string>? GetExamples(Question question)
    {
        if (question is not CodeWritingQuestion) return null;

        try
        {
            var metadata = JsonSerializer.Deserialize<CodeWritingMetadata>(question.Metadata);
            return metadata?.Examples;
        }
        catch
        {
            return null;
        }
    }

    private static ICollection<TestCaseResponse>? GetTestCases(Question question)
    {
        if (question is not CodeWritingQuestion) return null;

        try
        {
            var metadata = JsonSerializer.Deserialize<CodeWritingMetadata>(question.Metadata);
            return metadata?.TestCases.Select(tc => new TestCaseResponse
            {
                Input = tc.Input,
                ExpectedOutput = tc.ExpectedOutput
            }).ToList();
        }
        catch
        {
            return null;
        }
    }
} 