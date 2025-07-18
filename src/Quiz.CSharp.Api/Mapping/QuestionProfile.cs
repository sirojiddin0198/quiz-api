namespace Quiz.CSharp.Api.Mapping;

using AutoMapper;
using Quiz.CSharp.Api.Contracts;
using Quiz.CSharp.Data.Entities;

public sealed class QuestionProfile : Profile
{
    public QuestionProfile()
    {
        CreateMap<Question, QuestionResponse>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => GetQuestionType(src)))
            .ForMember(dest => dest.Metadata, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => GetMCQOptions(src)))
            .ForMember(dest => dest.Hints, opt => opt.MapFrom(src => src.Hints.OrderBy(h => h.OrderIndex).Select(h => h.Hint)))
            .ForMember(dest => dest.Explanation, opt => opt.MapFrom(src => GetExplanation(src)));

        CreateMap<Question, QuestionMetadata>()
            .ForMember(dest => dest.CollectionId, opt => opt.MapFrom(src => src.CollectionId))
            .ForMember(dest => dest.CollectionCode, opt => opt.MapFrom(src => src.Collection.Code))
            .ForMember(dest => dest.EstimatedTime, opt => opt.MapFrom(src => src.EstimatedTimeMinutes));

        CreateMap<Question, QuestionContent>()
            .ForMember(dest => dest.Examples, opt => opt.Ignore())
            .ForMember(dest => dest.TestCases, opt => opt.MapFrom(src => GetTestCases(src)));

        CreateMap<MCQOption, MCQOptionResponse>();
        CreateMap<TestCase, TestCaseResponse>();
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

    private static ICollection<MCQOption>? GetMCQOptions(Question question)
    {
        return question is MCQQuestion mcqQuestion ? mcqQuestion.Options : null;
    }

    private static ICollection<TestCase>? GetTestCases(Question question)
    {
        return question is CodeWritingQuestion codeWritingQuestion ? codeWritingQuestion.TestCases : null;
    }

    private static string? GetExplanation(Question question)
    {
        var firstHint = question.Hints.OrderBy(h => h.OrderIndex).FirstOrDefault();
        return firstHint?.Hint;
    }
} 