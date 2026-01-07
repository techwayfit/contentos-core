using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Abstractions.Filtering;
using TechWayFit.ContentOS.Abstractions.Pagination;
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories;

/// <summary>
/// Base EF Core repository providing common CRUD operations
/// Provider-agnostic - works with any EF Core provider (PostgreSQL, SQL Server, SQLite, etc.)
/// Maps between domain entities (TEntity) and database row entities (TRow)
/// </summary>
/// <typeparam name="TEntity">Domain entity type</typeparam>
/// <typeparam name="TRow">Database row entity type</typeparam>
/// <typeparam name="TKey">Primary key type</typeparam>
public abstract class EfCoreRepository<TEntity, TRow, TKey> : IRepository<TEntity, TKey>
    where TEntity : class
    where TRow : class
{
    protected readonly DbContext Context;

    protected EfCoreRepository(DbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    // Mapping methods that derived classes must implement
    protected abstract TRow MapToRow(TEntity entity);
    protected abstract TEntity MapToDomain(TRow row);
    protected abstract Expression<Func<TRow, bool>> CreateRowPredicate(TKey id);

    /// <summary>
    /// Gets the ID from a row entity
    /// Default implementation works for BaseEntity-derived rows
    /// Override if your TRow doesn't inherit from BaseEntity
    /// </summary>
    protected virtual Guid GetRowId(TRow row)
    {
        if (row is BaseEntity baseEntity)
            return baseEntity.Id;
        
        throw new NotImplementedException(
            $"GetRowId must be overridden for {typeof(TRow).Name} which doesn't inherit from BaseEntity");
    }

    // ==================== AUDIT MAPPING HELPERS (DRY) ====================
    
    /// <summary>
    /// Maps BaseEntity audit fields to AuditInfo domain object
    /// Use this in MapToDomain to avoid repeating audit field mapping
    /// </summary>
    protected static AuditInfo MapAuditToDomain(BaseEntity row)
    {
        return new AuditInfo
        {
            CreatedOn = row.CreatedOn,
            CreatedBy = row.CreatedBy,
            UpdatedOn = row.UpdatedOn,
            UpdatedBy = row.UpdatedBy,
            DeletedOn = row.DeletedOn,
            DeletedBy = row.DeletedBy,
            IsDeleted = row.IsDeleted,
            IsActive = row.IsActive,
            CanDelete = row.CanDelete,
            IsSystem = row.IsSystem
        };
    }

    /// <summary>
    /// Maps AuditInfo domain object to BaseEntity audit fields
    /// Use this in MapToRow to avoid repeating audit field mapping
    /// </summary>
    protected static void MapAuditToRow(AuditInfo audit, BaseEntity row)
    {
        row.CreatedOn = audit.CreatedOn;
        row.CreatedBy = audit.CreatedBy;
        row.UpdatedOn = audit.UpdatedOn;
        row.UpdatedBy = audit.UpdatedBy;
        row.DeletedOn = audit.DeletedOn;
        row.DeletedBy = audit.DeletedBy;
        row.IsDeleted = audit.IsDeleted;
        row.IsActive = audit.IsActive;
        row.CanDelete = audit.CanDelete;
        row.IsSystem = audit.IsSystem;
    }

    // ==================== BASIC CRUD ====================

    public virtual async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var row = await Context.Set<TRow>()
            .AsNoTracking()
            .FirstOrDefaultAsync(CreateRowPredicate(id), cancellationToken);

        return row == null ? null : MapToDomain(row);
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetByIdsAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        var rows = await Context.Set<TRow>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // Filter in memory after loading (can be optimized with specific queries in derived classes)
        var filtered = rows.Where(row => idList.Contains((TKey)(object)GetRowId(row))).ToList();
        return filtered.Select(MapToDomain).ToList();
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var rows = await Context.Set<TRow>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return rows.Select(MapToDomain).ToList();
    }

    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var row = MapToRow(entity);
        await Context.Set<TRow>().AddAsync(row, cancellationToken);
        // Note: SaveChanges is called by UnitOfWork, not here
    }

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var rows = entities.Select(MapToRow);
        await Context.Set<TRow>().AddRangeAsync(rows, cancellationToken);
    }

    public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var row = MapToRow(entity);
        Context.Set<TRow>().Update(row);
        return Task.CompletedTask;
    }

    public virtual Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var rows = entities.Select(MapToRow);
        Context.Set<TRow>().UpdateRange(rows);
        return Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var row = await Context.Set<TRow>()
            .FirstOrDefaultAsync(CreateRowPredicate(id), cancellationToken);

        if (row != null)
        {
            Context.Set<TRow>().Remove(row);
        }
    }

    public virtual async Task DeleteRangeAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        var rows = await Context.Set<TRow>().ToListAsync(cancellationToken);
        var toDelete = rows.Where(row => idList.Contains((TKey)(object)GetRowId(row))).ToList();

        if (toDelete.Any())
        {
            Context.Set<TRow>().RemoveRange(toDelete);
        }
    }

    // ==================== QUERYING ====================

    public virtual Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("FindAsync requires specific implementation in derived class");
    }

    public virtual Task<IReadOnlyList<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("FindAllAsync requires specific implementation in derived class");
    }

    public virtual Task<IReadOnlyList<TEntity>> FindWithSpecificationAsync(FilterSpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("FindWithSpecificationAsync requires specific implementation in derived class");
    }

    public virtual Task<PagedResult<TEntity>> FindPagedAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        PaginationParameters? pagination = null,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool ascending = true,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("FindPagedAsync requires specific implementation in derived class");
    }

    // ==================== AGGREGATION ====================

    public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        return await Context.Set<TRow>().CountAsync(cancellationToken);
    }

    public virtual Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("AnyAsync requires specific implementation in derived class");
    }

    public virtual async Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<TRow>().AnyAsync(CreateRowPredicate(id), cancellationToken);
    }

    // ==================== SEARCH ====================

    public virtual Task<IReadOnlyList<TEntity>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("SearchAsync requires specific implementation in derived class");
    }

    public virtual Task<PagedResult<TEntity>> SearchPagedAsync(string searchTerm, PaginationParameters pagination, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("SearchPagedAsync requires specific implementation in derived class");
    }
}
