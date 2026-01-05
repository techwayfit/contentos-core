using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Collaboration;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Collaboration;

public sealed class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLogRow>
{
    public void Configure(EntityTypeBuilder<ActivityLogRow> builder)
    {
        builder.ToTable("activity_log");
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(e => e.ModuleId).HasColumnName("module_id").IsRequired();
        builder.Property(e => e.EntityInstanceId).HasColumnName("entity_instance_id").IsRequired();
        builder.Property(e => e.ActivityType).HasColumnName("activity_type").HasMaxLength(100).IsRequired();
        builder.Property(e => e.ActivityKey).HasColumnName("activity_key").HasMaxLength(200).IsRequired();
        builder.Property(e => e.ActorUserId).HasColumnName("actor_user_id").IsRequired();
        builder.Property(e => e.FieldChanged).HasColumnName("field_changed").HasMaxLength(200).IsRequired();
        builder.Property(e => e.OldValue).HasColumnName("old_value").IsRequired();
        builder.Property(e => e.NewValue).HasColumnName("new_value").IsRequired();
        builder.Property(e => e.OccurredOn).HasColumnName("occurred_on").IsRequired();

        builder.HasOne(e => e.Module)
            .WithMany()
            .HasForeignKey(e => e.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Actor)
            .WithMany()
            .HasForeignKey(e => e.ActorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.TenantId, e.EntityInstanceId, e.OccurredOn }).HasDatabaseName("ix_activity_log_entity_occurred");
        builder.HasIndex(e => new { e.TenantId, e.ActorUserId, e.OccurredOn }).HasDatabaseName("ix_activity_log_actor_occurred");
    }
}
