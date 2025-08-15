namespace Quiz.CSharp.Api.Mapping;

using AutoMapper;
using Quiz.CSharp.Api.Contracts;
using Quiz.CSharp.Data.Entities;

public sealed class CollectionProfile : Profile
{
    public CollectionProfile()
    {
        CreateMap<Collection, CollectionResponse>()
            .ForMember(dest => dest.UserProgress, opt => opt.Ignore());

        CreateMap<UserProgress, UserProgressResponse>()
            .ForMember(dest => dest.CompletionRate, opt => opt.MapFrom(src =>
                src.TotalQuestions > 0 ? (decimal)src.AnsweredQuestions / src.TotalQuestions * 100 : 0));

        CreateMap<UserProgress, UserProgressManagementResponse>()
            .ForMember(dest => dest.CollectionCode, opt => opt.MapFrom(src => src.Collection.Code))
            .ForMember(dest => dest.CollectionTitle, opt => opt.MapFrom(src => src.Collection.Title))
            .ForMember(dest => dest.CompletionRate, opt => opt.MapFrom(src =>
                src.TotalQuestions > 0 ? (decimal)src.AnsweredQuestions / src.TotalQuestions * 100 : 0));

        CreateMap<UserProgress, CollectionProgressResponse>()
            .ForMember(dest => dest.CollectionCode, opt => opt.MapFrom(src => src.Collection.Code))
            .ForMember(dest => dest.CollectionTitle, opt => opt.MapFrom(src => src.Collection.Title))
            .ForMember(dest => dest.CollectionDescription, opt => opt.MapFrom(src => src.Collection.Description))
            .ForMember(dest => dest.CompletionRate, opt => opt.MapFrom(src =>
                src.TotalQuestions > 0 ? (decimal)src.AnsweredQuestions / src.TotalQuestions * 100 : 0));

        CreateMap<(ICurrentUser currentUser, string UserId, int CollectionId, int Total, int Answered, int Correct, decimal Rate), UserProgress>()
            .ForMember(d => d.UserId, m => m.MapFrom(s => s.UserId))
            .ForMember(d => d.Username, m => m.MapFrom(s => s.currentUser.Username))
            .ForMember(d => d.Name, m => m.MapFrom(s => s.currentUser.Name))
            .ForMember(d => d.TelegramUsername, m => m.MapFrom(s => s.currentUser.TelegramUsername))
            .ForMember(d => d.CollectionId, m => m.MapFrom(s => s.CollectionId))
            .ForMember(d => d.TotalQuestions, m => m.MapFrom(s => s.Total))
            .ForMember(d => d.AnsweredQuestions, m => m.MapFrom(s => s.Answered))
            .ForMember(d => d.CorrectAnswers, m => m.MapFrom(s => s.Correct))
            .ForMember(d => d.SuccessRate, m => m.MapFrom(s => Math.Round(s.Rate, 2)))
            .ForMember(d => d.LastAnsweredAt, m => m.MapFrom(_ => DateTime.UtcNow))
            .ForMember(d => d.CreatedAt, m => m.MapFrom(_ => DateTime.UtcNow))
            .ForMember(d => d.IsActive, m => m.MapFrom(_ => true));

        CreateMap<CreateCollectionRequest, Collection>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.Questions, opt => opt.Ignore())
            .ForMember(dest => dest.UserProgress, opt => opt.Ignore());

        CreateMap<Collection, CreateCollectionResponse>()
            .ForMember(dest => dest.QuestionsCreated, opt => opt.Ignore());
    }
}