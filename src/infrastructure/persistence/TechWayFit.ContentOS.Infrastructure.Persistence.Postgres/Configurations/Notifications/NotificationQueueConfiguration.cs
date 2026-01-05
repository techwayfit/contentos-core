using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Notifications;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Notifications;

public sealed class NotificationQueueConfiguration : IEntityTypeConfiguration<NotificationQueueRow>
{
    public void Configure(EntityTypeBuilder<NotificationQueueRow> builder)
    {
        builder.ToTable("notification_queue");
        builder.HasKey(e => e.Id);
        builder.ConfigureTenantKey();
        
        builder.Property(e => e.TemplateId).HasColumnName("template_id").IsRequired();
        builder.Property(e => e.RecipientEmail).HasColumnName("recipient_email").HasMaxLength(500).IsRequired();
        builder.Property(e => e.PayloadJson).HasColumnName("payload_json").HasColumnType("jsonb").IsRequired();
        builder.Property(e => e.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
        builder.Property(e => e.ScheduledFor).HasColumnName("scheduled_for").IsRequired();
        builder.Property(e => e.SentAt).HasColumnName("sent_at");
        builder.Property(e => e.ErrorMessage).HasColumnName("error_message");

        builder.ConfigureAuditFields();

        builder.HasOne(e => e.Template)
            .WithMany()
            .HasForeignKey(e => e.TemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.TenantId, e.Status, e.ScheduledFor }).HasDatabaseName("ix_notification_queue_status_scheduled");
    }
}
