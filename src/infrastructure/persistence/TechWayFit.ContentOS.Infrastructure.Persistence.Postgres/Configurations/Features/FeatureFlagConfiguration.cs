using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Features;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Features;

public sealed class FeatureFlagConfiguration : IEntityTypeConfiguration<FeatureFlagRow>
{
    public void Configure(EntityTypeBuilder<FeatureFlagRow> builder)
    {
        builder.ToTable("feature_flag");
        builder.HasKey(e => e.Id);
        builder.ConfigureTenantKey();
        
        builder.Property(e => e.FeatureKey).HasColumnName("feature_key").HasMaxLength(200).IsRequired();
        builder.Property(e => e.IsEnabled).HasColumnName("is_enabled").IsRequired();
        builder.Property(e => e.RulesJson).HasColumnName("rules_json").HasColumnType("jsonb").IsRequired();

        builder.ConfigureAuditFields();

        builder.HasIndex(e => new { e.TenantId, e.FeatureKey })
            .IsUnique()
            .HasFilter("deleted_on IS NULL")
            .HasDatabaseName("uq_feature_flag_tenant_key");
    }
}
