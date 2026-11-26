namespace BlogService.Application.Services.Mapping;

using AutoMapper;
using BlogService.Application.Models.Article;
using BlogService.Application.Models.Author;
using BlogService.Application.Models.Comment;
using ArticleEntity = BlogService.Domain.Entities.Article;
using AuthorEntity = BlogService.Domain.Entities.Author;
using CommentEntity = BlogService.Domain.Entities.Comment;

public class ApplicationProfile : Profile
{
    public ApplicationProfile()
    {
        CreateMap<AuthorEntity, AuthorModel>();

        CreateMap<ArticleEntity, ArticleModel>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title.Value))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content.Value));

        CreateMap<CommentEntity, CommentModel>()
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text.Value));
    }
}
