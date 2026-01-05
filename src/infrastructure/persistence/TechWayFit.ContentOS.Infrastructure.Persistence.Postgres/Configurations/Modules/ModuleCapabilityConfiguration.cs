using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Modules;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Modules;

public sealed class ModuleCapabilityConfiguration : IEntityTypeConfiguration<ModuleCapabilityRow>
{
    public void Configure(EntityTypeBuilder<ModuleCapabilityRow> builder)
    {
        builder.ToTable("module_capability");
        builder.HasKey(e => e.Id);
        builder.ConfigureTenantKey();
        
        builder.Property(e => e.ModuleId).HasColumnName("module_id").IsRequired();
        builder.Property(e => e.CapabilityKey).HasColumnName("capability_key").HasMaxLength(200).IsRequired();
        builder.Property(e => e.Description).HasColumnName("description").HasMaxLength(500).IsRequired();
        builder.Property(e => e.RequiredByDependents).HasColumnName("required_by_dependents").IsRequired();

        builder.ConfigureAuditFields();

        builder.HasOne(e => e.Module)
            .WithMany()
            .HasForeignKey(e => e.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.TenantId, e.ModuleId, e.CapabilityKey })
            .IsUnique()
            .HasFilter("deleted_on IS NULL")
            .HasDatabaseName("uq_module_capability_tenant_module_key");
    }
}
