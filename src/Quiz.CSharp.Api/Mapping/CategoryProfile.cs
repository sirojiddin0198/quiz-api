namespace Quiz.CSharp.Api.Mapping;

using AutoMapper;
using Quiz.CSharp.Api.Contracts;
using Quiz.CSharp.Data.Entities;

public sealed class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryResponse>()
            .ForMember(dest => dest.TotalQuestions, opt => opt.MapFrom(src => src.Questions.Count(q => q.IsActive)));
    }
} 