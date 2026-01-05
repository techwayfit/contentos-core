using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Modules;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Modules;

public sealed class ModuleMigrationConfiguration : IEntityTypeConfiguration<ModuleMigrationRow>
{
    public void Configure(EntityTypeBuilder<ModuleMigrationRow> builder)
    {
        builder.ToTable("module_migration");
        builder.HasKey(e => e.Id);
        builder.ConfigureTenantKey();
        
        builder.Property(e => e.ModuleId).HasColumnName("module_id").IsRequired();
        builder.Property(e => e.MigrationKey).HasColumnName("migration_key").HasMaxLength(200).IsRequired();
        builder.Property(e => e.AppliedAt).HasColumnName("applied_at").IsRequired();
        builder.Property(e => e.AppliedByUserId).HasColumnName("applied_by_user_id").IsRequired();

        builder.ConfigureAuditFields();

        builder.HasOne(e => e.Module)
            .WithMany()
            .HasForeignKey(e => e.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.TenantId, e.ModuleId, e.MigrationKey })
            .IsUnique()
            .HasDatabaseName("uq_module_migration_tenant_module_key");
    }
}
