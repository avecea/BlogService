namespace BlogService.Infrastructure.EntityFramework.Repositories;

using BlogService.Domain.Entities;
using BlogService.Domain.Enums;
using BlogService.Domain.Repositories.Abstractions;
using BlogService.Infrastructure.EntityFramework.Data;
using Microsoft.EntityFrameworkCore;

public class ArticleRepository : IArticleRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<Article> _dbSet;

    public ArticleRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<Article>();
    }

    public async Task<Article?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _dbSet
            .Include(a => a.Author)
            .FirstOrDefaultAsync(a => a.Id == id, ct);
    }

    public async Task<IReadOnlyList<Article>> GetAllAsync(CancellationToken ct)
    {
        return await _dbSet
            .Include(a => a.Author)
            .ToListAsync(ct);
    }

    public async Task<Article> AddAsync(Article entity, CancellationToken ct)
    {
        await _dbSet.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task UpdateAsync(Article entity, CancellationToken ct)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Article entity, CancellationToken ct)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<Article>> GetByAuthorIdAsync(Guid authorId, CancellationToken ct)
    {
        return await _dbSet
            .Where(a => a.AuthorId == authorId)
            .Include(a => a.Author)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Article>> GetPublishedAsync(CancellationToken ct)
    {
        return await _dbSet
            .Where(a => a.Status == ArticleStatus.Published)
            .Include(a => a.Author)
            .ToListAsync(ct);
    }
}