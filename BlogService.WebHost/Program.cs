using AutoMapper;
using BlogService.Application.Services;
using BlogService.Application.Services.Abstractions;
using BlogService.Application.Services.Mapping;
using BlogService.Domain.Repositories.Abstractions;
using BlogService.Infrastructure.EntityFramework.Data;
using BlogService.Infrastructure.EntityFramework.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

// Application
var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<ApplicationProfile>());
builder.Services.AddSingleton<IMapper>(mapperConfiguration.CreateMapper());
builder.Services.AddScoped<IAuthorsApplicationService, AuthorsApplicationService>();
builder.Services.AddScoped<IArticlesApplicationService, ArticlesApplicationService>();
builder.Services.AddScoped<ICommentsApplicationService, CommentsApplicationService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
