using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Collaboration;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Collaboration;

public sealed class AttachmentConfiguration : IEntityTypeConfiguration<AttachmentRow>
{
    public void Configure(EntityTypeBuilder<AttachmentRow> builder)
    {
        builder.ToTable("attachment");
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(e => e.ModuleId).HasColumnName("module_id").IsRequired();
        builder.Property(e => e.EntityInstanceId).HasColumnName("entity_instance_id").IsRequired();
        builder.Property(e => e.FileName).HasColumnName("file_name").HasMaxLength(500).IsRequired();
        builder.Property(e => e.FileSizeBytes).HasColumnName("file_size_bytes").IsRequired();
        builder.Property(e => e.MimeType).HasColumnName("mime_type").HasMaxLength(100).IsRequired();
        builder.Property(e => e.StoragePath).HasColumnName("storage_path").HasMaxLength(1000).IsRequired();
        builder.Property(e => e.ScanStatus).HasColumnName("scan_status").HasMaxLength(50).IsRequired();
        builder.Property(e => e.CreatedOn).HasColumnName("created_on").IsRequired();
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").IsRequired();

        builder.HasOne(e => e.Module)
            .WithMany()
            .HasForeignKey(e => e.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.TenantId, e.EntityInstanceId }).HasDatabaseName("ix_attachment_entity");
    }
}
