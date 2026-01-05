using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Notifications;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Notifications;

public sealed class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplateRow>
{
    public void Configure(EntityTypeBuilder<NotificationTemplateRow> builder)
    {
        builder.ToTable("notification_template");
        builder.HasKey(e => e.Id);
        builder.ConfigureTenantKey();
        
        builder.Property(e => e.TemplateKey).HasColumnName("template_key").HasMaxLength(200).IsRequired();
        builder.Property(e => e.Channel).HasColumnName("channel").HasMaxLength(50).IsRequired();
        builder.Property(e => e.Subject).HasColumnName("subject").HasMaxLength(500).IsRequired();
        builder.Property(e => e.BodyTemplate).HasColumnName("body_template").IsRequired();

        builder.ConfigureAuditFields();

        builder.HasIndex(e => new { e.TenantId, e.TemplateKey, e.Channel })
            .IsUnique()
            .HasFilter("deleted_on IS NULL")
            .HasDatabaseName("uq_notification_template_tenant_key_channel");
    }
}
