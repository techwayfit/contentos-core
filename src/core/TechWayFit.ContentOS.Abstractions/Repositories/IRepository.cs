using System.Linq.Expressions;
using TechWayFit.ContentOS.Abstractions.Filtering;
using TechWayFit.ContentOS.Abstractions.Pagination;

namespace TechWayFit.ContentOS.Abstractions.Repositories;

/// <summary>
/// Base repository interface providing standard CRUD operations
/// All repositories must inherit from this interface
/// </summary>
/// <typeparam name="TEntity">Domain entity type (must be a class)</typeparam>
/// <typeparam name="TKey">Primary key type</typeparam>
public interface IRepository<TEntity, TKey> where TEntity : class
{
    // ==================== BASIC CRUD ====================

    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> GetByIdsAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task DeleteAsync(TKey id, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default);

    // ==================== QUERYING ====================

    Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> FindWithSpecificationAsync(FilterSpecification<TEntity> specification, CancellationToken cancellationToken = default);
    
    Task<PagedResult<TEntity>> FindPagedAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        PaginationParameters? pagination = null,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool ascending = true,
        CancellationToken cancellationToken = default);

    // ==================== AGGREGATION ====================

    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default);

    // ==================== SEARCH ====================

    Task<IReadOnlyList<TEntity>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<PagedResult<TEntity>> SearchPagedAsync(string searchTerm, PaginationParameters pagination, CancellationToken cancellationToken = default);
}
