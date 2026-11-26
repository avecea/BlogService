namespace BlogService.Infrastructure.EntityFramework.Configurations;

using BlogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.ToTable("articles");

        builder.HasKey(a => a.Id);

        builder.OwnsOne(a => a.Title, tb =>
        {
            tb.Property(t => t.Value)
                .HasColumnName("title")
                .IsRequired()
                .HasMaxLength(200);
        });

        builder.OwnsOne(a => a.Content, cb =>
        {
            cb.Property(c => c.Value)
                .HasColumnName("content")
                .IsRequired();
        });

        builder.Property(a => a.Status)
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.HasOne(a => a.Author)
            .WithMany(auth => auth.Articles)
            .HasForeignKey(a => a.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}