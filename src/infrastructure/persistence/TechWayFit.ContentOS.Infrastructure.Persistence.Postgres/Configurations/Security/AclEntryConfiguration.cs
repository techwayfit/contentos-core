using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Security;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Security;

public sealed class AclEntryConfiguration : IEntityTypeConfiguration<AclEntryRow>
{
    public void Configure(EntityTypeBuilder<AclEntryRow> builder)
    {
        builder.ToTable("acl_entry");
        builder.HasKey(e => e.Id);
        builder.ConfigureTenantKey();
        
        builder.Property(e => e.ScopeType).HasColumnName("scope_type").HasMaxLength(50).IsRequired();
        builder.Property(e => e.ScopeId).HasColumnName("scope_id").IsRequired();
        builder.Property(e => e.PrincipalType).HasColumnName("principal_type").HasMaxLength(50).IsRequired();
        builder.Property(e => e.PrincipalId).HasColumnName("principal_id").IsRequired();
        builder.Property(e => e.Effect).HasColumnName("effect").HasMaxLength(20).IsRequired();
        builder.Property(e => e.ActionsCsv).HasColumnName("actions_csv").HasMaxLength(500).IsRequired();

        builder.ConfigureAuditFields();

        builder.HasIndex(e => new { e.TenantId, e.ScopeType, e.ScopeId }).HasDatabaseName("ix_acl_entry_scope");
        builder.HasIndex(e => new { e.TenantId, e.PrincipalType, e.PrincipalId }).HasDatabaseName("ix_acl_entry_principal");
    }
}
