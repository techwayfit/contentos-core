# Repository Architecture Recommendations

## Decision: Use UnitOfWork Pattern ✅

### Rationale

After reviewing the ADR documents and existing architecture:

1. **UnitOfWork Already Exists**
   - `IUnitOfWork` is already defined in Abstractions
   - `EfUnitOfWork` is implemented in Persistence.Postgres
   - Use-cases in Content and Workflow modules already use it
   - Follows established ContentOS patterns

2. **Architectural Alignment**
   - **ADR-015** enforces strict separation of concerns
   - Abstractions define interfaces (IUnitOfWork, IRepository)
   - Infrastructure provides implementations
   - Domain/Feature projects use interfaces only

3. **Transaction Management**
   - Multiple repositories often need to work together in a single transaction
   - Example: Creating ContentItem + ContentVersion + ContentFieldValue atomically
   - UnitOfWork coordinates transactions across multiple aggregates

4. **Performance Benefits**
   - Single SaveChanges() call batches all modifications
   - Reduces round-trips to database
   - Better change tracking

## Implementation Pattern

### Use-Case Layer (Feature Projects)
```csharp
public class CreateContentUseCase
{
    private readonly IContentItemRepository _contentItemRepo;
    private readonly IContentVersionRepository _versionRepo;
    private readonly IContentFieldValueRepository _fieldValueRepo;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Result<Guid>> ExecuteAsync(CreateContentCommand cmd)
    {
        // 1. Business logic
        var contentItem = ContentItem.Create(...);
        var version = ContentVersion.CreateDraft(contentItem.Id, ...);
        
        // 2. Persist via repositories
        await _contentItemRepo.AddAsync(contentItem);
        await _versionRepo.AddAsync(version);
        
        foreach (var field in cmd.Fields)
        {
            var fieldValue = ContentFieldValue.Create(version.Id, field.Key, field.Value);
            await _fieldValueRepo.AddAsync(fieldValue);
        }
        
        // 3. Commit via UnitOfWork
        await _unitOfWork.SaveChangesAsync();
        
        return Result.Success(contentItem.Id);
    }
}
```

### Repository Layer
Repositories **do NOT** call SaveChanges() internally. They only modify the DbContext's change tracker.

```csharp
public class ContentItemRepository : BaseRepository<ContentItem, ContentItemRow, Guid>
{
    // AddAsync does NOT call SaveChanges
    // UpdateAsync does NOT call SaveChanges
    // DeleteAsync does NOT call SaveChanges
    
    // Only UnitOfWork calls SaveChanges
}
```

## Updated BaseRepository Design

### Provider-Agnostic Base (Persistence)
- Defines abstract methods
- Implements common patterns
- **Does NOT call SaveChanges**

### EF Core Base (Persistence.Postgres)
- Implements EF-specific methods
- Uses DbContext
- **Does NOT call SaveChanges**
- Let UnitOfWork handle transactions

### Specific Repositories
- Inherit from EF Core base
- Add domain-specific queries
- **Never call SaveChanges**

## Repository Hierarchy

```
IRepository<TEntity, TKey> (Abstractions)
    ↓
BaseRepository<TEntity, TRow, TKey> (Persistence - provider-agnostic)
    ↓
EfCoreBaseRepository<TEntity, TRow, TKey> (Persistence.Postgres - EF-specific)
    ↓
ContentItemRepository : EfCoreBaseRepository<ContentItem, ContentItemRow, Guid>
UserRepository : EfCoreBaseRepository<User, UserRow, Guid>
[etc.]
```

## Benefits

1. **Transaction Control**: Use-cases control transaction boundaries
2. **Performance**: Batch multiple operations
3. **Testability**: Easy to mock IUnitOfWork
4. **Consistency**: Follows existing ContentOS patterns
5. **Flexibility**: Can begin/commit/rollback transactions explicitly

## Alternative Considered: Independent Repositories

**Rejected because:**
- Would require each repository to call SaveChanges
- Makes cross-aggregate transactions difficult
- Conflicts with existing UnitOfWork pattern
- Use-cases lose transaction control
- Higher database round-trips

## Migration Path

1. ✅ Fix BaseRepository to NOT call SaveChanges
2. ✅ Create EfCoreBaseRepository 
3. ✅ Migrate existing repositories to new pattern
4. Create remaining 35+ repositories
5. Update use-cases to explicitly use UnitOfWork

## Summary

**Use UnitOfWork Pattern** - It's already established in ContentOS and aligns with:
- ADR-015 (Abstractions/Infrastructure separation)
- Use-case-driven transaction boundaries
- Enterprise transaction patterns
- Existing codebase conventions
