using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Processes;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Processes;

public sealed class ProcessDefinitionConfiguration : IEntityTypeConfiguration<ProcessDefinitionRow>
{
    public void Configure(EntityTypeBuilder<ProcessDefinitionRow> builder)
    {
        builder.ToTable("process_definition");
        builder.HasKey(e => e.Id);
        builder.ConfigureTenantKey();
        
        builder.Property(e => e.ModuleId).HasColumnName("module_id").IsRequired();
        builder.Property(e => e.ProcessKey).HasColumnName("process_key").HasMaxLength(200).IsRequired();
        builder.Property(e => e.ProcessType).HasColumnName("process_type").HasMaxLength(100).IsRequired();
        builder.Property(e => e.StatesJson).HasColumnName("states_json").HasColumnType("jsonb").IsRequired();
        builder.Property(e => e.TransitionsJson).HasColumnName("transitions_json").HasColumnType("jsonb").IsRequired();
        builder.Property(e => e.HasSla).HasColumnName("has_sla").IsRequired();

        builder.ConfigureAuditFields();

        builder.HasOne(e => e.Module)
            .WithMany()
            .HasForeignKey(e => e.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.TenantId, e.ModuleId, e.ProcessKey })
            .IsUnique()
            .HasFilter("deleted_on IS NULL")
            .HasDatabaseName("uq_process_definition_tenant_key_version");
    }
}
