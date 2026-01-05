using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Preview;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Preview;

public sealed class PreviewTokenConfiguration : IEntityTypeConfiguration<PreviewTokenRow>
{
    public void Configure(EntityTypeBuilder<PreviewTokenRow> builder)
    {
        builder.ToTable("preview_token");
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(e => e.SiteId).HasColumnName("site_id").IsRequired();
        builder.Property(e => e.NodeId).HasColumnName("node_id").IsRequired();
        builder.Property(e => e.ContentVersionId).HasColumnName("content_version_id").IsRequired();
        builder.Property(e => e.TokenHash).HasColumnName("token_hash").HasMaxLength(64).IsRequired();
        builder.Property(e => e.ExpiresAt).HasColumnName("expires_at").IsRequired();
        builder.Property(e => e.IssuedToEmail).HasColumnName("issued_to_email").HasMaxLength(255);
        builder.Property(e => e.OneTimeUse).HasColumnName("one_time_use").IsRequired();
        builder.Property(e => e.UsedAt).HasColumnName("used_at");
        builder.Property(e => e.CreatedOn).HasColumnName("created_on").IsRequired();
        builder.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired();
        builder.Property(e => e.IsActive).HasColumnName("is_active").IsRequired();

        builder.HasOne(e => e.Site)
            .WithMany()
            .HasForeignKey(e => e.SiteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Node)
            .WithMany()
            .HasForeignKey(e => e.NodeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.ContentVersion)
            .WithMany()
            .HasForeignKey(e => e.ContentVersionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.TenantId, e.TokenHash })
            .IsUnique()
            .HasDatabaseName("uq_preview_token_tenant_hash");
    }
}
