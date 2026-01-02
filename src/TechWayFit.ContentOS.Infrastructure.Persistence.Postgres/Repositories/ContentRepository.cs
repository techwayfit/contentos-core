using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Content.Domain;
using TechWayFit.ContentOS.Content.Ports;
using TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Mappers;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Repositories;

/// <summary>
/// Repository implementation for ContentItem using EF Core
/// </summary>
public class ContentRepository : IContentRepository
{
    private readonly ContentOsDbContext _context;

    public ContentRepository(ContentOsDbContext context)
    {
        _context = context;
    }

    public async Task<ContentItem?> GetByIdAsync(ContentItemId id, CancellationToken cancellationToken = default)
    {
        var row = await _context.ContentItems
            .Include(x => x.Localizations)
            .FirstOrDefaultAsync(x => x.Id == id.Value, cancellationToken);

        return row == null ? null : ContentItemMapper.ToDomain(row);
    }

    public async Task<PagedResult<ContentItem>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ContentItems
            .Include(x => x.Localizations)
            .OrderByDescending(x => x.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var rows = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = rows.Select(ContentItemMapper.ToDomain).ToList();

        return new PagedResult<ContentItem>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task AddAsync(ContentItem content, CancellationToken cancellationToken = default)
    {
        var row = ContentItemMapper.ToRow(content);
        await _context.ContentItems.AddAsync(row, cancellationToken);
    }

    public Task UpdateAsync(ContentItem content, CancellationToken cancellationToken = default)
    {
        var row = _context.ContentItems
            .Include(x => x.Localizations)
            .FirstOrDefault(x => x.Id == content.Id.Value);

        if (row == null)
            throw new InvalidOperationException($"Content item {content.Id} not found for update");

        ContentItemMapper.UpdateRow(row, content);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(ContentItemId id, CancellationToken cancellationToken = default)
    {
        var row = await _context.ContentItems.FindAsync(new object[] { id.Value }, cancellationToken);
        if (row != null)
        {
            _context.ContentItems.Remove(row);
        }
    }
}
