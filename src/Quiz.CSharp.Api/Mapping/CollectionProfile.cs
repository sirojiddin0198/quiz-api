namespace Quiz.CSharp.Api.Mapping;

using AutoMapper;
using Quiz.CSharp.Api.Contracts;
using Quiz.CSharp.Api.Dtos;
using Quiz.CSharp.Data.Entities;

public sealed class CollectionProfile : Profile
{
    public CollectionProfile()
    {
        CreateMap<Collection, CollectionResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Icon, opt => opt.MapFrom(src => src.Icon))
            .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => src.SortOrder))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.UserProgress, opt => opt.Ignore());

        CreateMap<UserProgress, UserProgressResponse>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.TelegramUsername, opt => opt.MapFrom(src => src.TelegramUsername))
            .ForMember(dest => dest.AnsweredQuestions, opt => opt.MapFrom(src => src.AnsweredQuestions))
            .ForMember(dest => dest.CorrectAnswers, opt => opt.MapFrom(src => src.CorrectAnswers))
            .ForMember(dest => dest.SuccessRate, opt => opt.MapFrom(src => src.SuccessRate))
            .ForMember(dest => dest.CompletionRate, opt => opt.MapFrom(src =>
                src.TotalQuestions > 0 ? (decimal)src.AnsweredQuestions / src.TotalQuestions * 100 : 0));

        CreateMap<UserProgress, UserProgressManagementResponse>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.TelegramUsername, opt => opt.MapFrom(src => src.TelegramUsername))
            .ForMember(dest => dest.CollectionId, opt => opt.MapFrom(src => src.CollectionId))
            .ForMember(dest => dest.CollectionCode, opt => opt.MapFrom(src => src.Collection.Code))
            .ForMember(dest => dest.CollectionTitle, opt => opt.MapFrom(src => src.Collection.Title))
            .ForMember(dest => dest.TotalQuestions, opt => opt.MapFrom(src => src.TotalQuestions))
            .ForMember(dest => dest.AnsweredQuestions, opt => opt.MapFrom(src => src.AnsweredQuestions))
            .ForMember(dest => dest.CorrectAnswers, opt => opt.MapFrom(src => src.CorrectAnswers))
            .ForMember(dest => dest.SuccessRate, opt => opt.MapFrom(src => src.SuccessRate))
            .ForMember(dest => dest.CompletionRate, opt => opt.MapFrom(src =>
                src.TotalQuestions > 0 ? (decimal)src.AnsweredQuestions / src.TotalQuestions * 100 : 0))
            .ForMember(dest => dest.LastAnsweredAt, opt => opt.MapFrom(src => src.LastAnsweredAt))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        CreateMap<UserProgress, CollectionProgressResponse>()
            .ForMember(dest => dest.CollectionId, opt => opt.MapFrom(src => src.CollectionId))
            .ForMember(dest => dest.CollectionCode, opt => opt.MapFrom(src => src.Collection.Code))
            .ForMember(dest => dest.CollectionTitle, opt => opt.MapFrom(src => src.Collection.Title))
            .ForMember(dest => dest.CollectionDescription, opt => opt.MapFrom(src => src.Collection.Description))
            .ForMember(dest => dest.TotalQuestions, opt => opt.MapFrom(src => src.TotalQuestions))
            .ForMember(dest => dest.AnsweredQuestions, opt => opt.MapFrom(src => src.AnsweredQuestions))
            .ForMember(dest => dest.CorrectAnswers, opt => opt.MapFrom(src => src.CorrectAnswers))
            .ForMember(dest => dest.SuccessRate, opt => opt.MapFrom(src => src.SuccessRate))
            .ForMember(dest => dest.CompletionRate, opt => opt.MapFrom(src =>
                src.TotalQuestions > 0 ? (decimal)src.AnsweredQuestions / src.TotalQuestions * 100 : 0))
            .ForMember(dest => dest.LastAnsweredAt, opt => opt.MapFrom(src => src.LastAnsweredAt))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        CreateMap<CreateCollection, Collection>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()); 
    }
} 