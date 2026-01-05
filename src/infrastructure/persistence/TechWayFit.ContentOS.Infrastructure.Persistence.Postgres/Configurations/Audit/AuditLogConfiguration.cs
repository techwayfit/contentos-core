using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Audit;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Audit;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLogRow>
{
    public void Configure(EntityTypeBuilder<AuditLogRow> builder)
    {
        builder.ToTable("audit_log");
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(e => e.ActorUserId).HasColumnName("actor_user_id");
        builder.Property(e => e.ActionKey).HasColumnName("action_key").HasMaxLength(200).IsRequired();
        builder.Property(e => e.EntityType).HasColumnName("entity_type").HasMaxLength(100).IsRequired();
        builder.Property(e => e.EntityId).HasColumnName("entity_id").IsRequired();
        builder.Property(e => e.DetailsJson).HasColumnName("details_json").HasColumnType("jsonb").IsRequired();
        builder.Property(e => e.CreatedOn).HasColumnName("created_on").IsRequired();

        builder.HasIndex(e => new { e.TenantId, e.CreatedOn }).HasDatabaseName("ix_audit_log_tenant_created");
        builder.HasIndex(e => new { e.TenantId, e.EntityType, e.EntityId }).HasDatabaseName("ix_audit_log_entity");
        builder.HasIndex(e => new { e.TenantId, e.ActorUserId, e.CreatedOn }).HasDatabaseName("ix_audit_log_actor");
    }
}
