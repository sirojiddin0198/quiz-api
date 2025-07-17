namespace Quiz.CSharp.Api.Mapping;

using AutoMapper;
using Quiz.CSharp.Api.Contracts;
using Quiz.CSharp.Data.Entities;

public sealed class CollectionProfile : Profile
{
    public CollectionProfile()
    {
        CreateMap<Collection, CollectionResponse>();
    }
} 