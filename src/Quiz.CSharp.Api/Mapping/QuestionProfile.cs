namespace Quiz.CSharp.Api.Mapping;

using AutoMapper;
using Quiz.CSharp.Api.Contracts;
using Quiz.CSharp.Data.Entities;

public sealed class QuestionProfile : Profile
{
    public QuestionProfile()
    {
        CreateMap<Question, QuestionResponse>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Metadata, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options))
            .ForMember(dest => dest.Hints, opt => opt.MapFrom(src => src.Hints.OrderBy(h => h.OrderIndex).Select(h => h.Hint)));

        CreateMap<Question, QuestionMetadata>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.CategoryId))
            .ForMember(dest => dest.EstimatedTime, opt => opt.MapFrom(src => src.EstimatedTimeMinutes));

        CreateMap<Question, QuestionContent>()
            .ForMember(dest => dest.Examples, opt => opt.Ignore())
            .ForMember(dest => dest.TestCases, opt => opt.MapFrom(src => src.TestCases));

        CreateMap<MCQOption, MCQOptionResponse>();
        CreateMap<TestCase, TestCaseResponse>();
    }
} 