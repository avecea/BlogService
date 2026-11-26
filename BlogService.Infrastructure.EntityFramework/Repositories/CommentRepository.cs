namespace BlogService.Infrastructure.EntityFramework.Repositories;

using BlogService.Domain.Entities;
using BlogService.Domain.Repositories.Abstractions;
using BlogService.Infrastructure.EntityFramework.Data;
using Microsoft.EntityFrameworkCore;

public class CommentRepository : ICommentRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<Comment> _dbSet;

    public CommentRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<Comment>();
    }

    public async Task<Comment?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _dbSet
            .Include(c => c.Article)
            .Include(c => c.Author)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<IReadOnlyList<Comment>> GetAllAsync(CancellationToken ct)
    {
        return await _dbSet
            .Include(c => c.Article)
            .Include(c => c.Author)
            .ToListAsync(ct);
    }

    public async Task<Comment> AddAsync(Comment entity, CancellationToken ct)
    {
        await _dbSet.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task UpdateAsync(Comment entity, CancellationToken ct)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Comment entity, CancellationToken ct)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<Comment>> GetByArticleIdAsync(Guid articleId, CancellationToken ct)
    {
        return await _dbSet
            .Where(c => c.ArticleId == articleId)
            .Include(c => c.Author)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Comment>> GetVisibleByArticleIdAsync(Guid articleId, CancellationToken ct)
    {
        return await _dbSet
            .Where(c => c.ArticleId == articleId && !c.IsHidden)
            .Include(c => c.Author)
            .ToListAsync(ct);
    }
}