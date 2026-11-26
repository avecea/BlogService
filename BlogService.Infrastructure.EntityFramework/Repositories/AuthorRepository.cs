namespace BlogService.Infrastructure.EntityFramework.Repositories;

using BlogService.Domain.Entities;
using BlogService.Domain.Repositories.Abstractions;
using BlogService.Infrastructure.EntityFramework.Data;
using Microsoft.EntityFrameworkCore;

public class AuthorRepository : IAuthorRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<Author> _dbSet;

    public AuthorRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<Author>();
    }

    public async Task<Author?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _dbSet.FindAsync(new object[] { id }, ct);
    }

    public async Task<IReadOnlyList<Author>> GetAllAsync(CancellationToken ct)
    {
        return await _dbSet.ToListAsync(ct);
    }

    public async Task<Author> AddAsync(Author entity, CancellationToken ct)
    {
        await _dbSet.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task UpdateAsync(Author entity, CancellationToken ct)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Author entity, CancellationToken ct)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<Author?> GetByEmailAsync(string email, CancellationToken ct)
    {
        return await _dbSet.FirstOrDefaultAsync(a => a.Email == email, ct);
    }
}