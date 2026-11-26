namespace BlogService.Domain.Repositories.Abstractions.Base;

using BlogService.Domain.Base;

public interface IRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : struct, IEquatable<TId>
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken ct);
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct);
    Task<TEntity> AddAsync(TEntity entity, CancellationToken ct);
    Task UpdateAsync(TEntity entity, CancellationToken ct);
    Task DeleteAsync(TEntity entity, CancellationToken ct);
}