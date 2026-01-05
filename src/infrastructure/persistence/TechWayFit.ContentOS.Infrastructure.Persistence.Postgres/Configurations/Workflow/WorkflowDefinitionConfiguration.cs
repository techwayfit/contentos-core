using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Workflow;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Workflow;

public sealed class WorkflowDefinitionConfiguration : IEntityTypeConfiguration<WorkflowDefinitionRow>
{
    public void Configure(EntityTypeBuilder<WorkflowDefinitionRow> builder)
    {
        builder.ToTable("workflow_definition");
        builder.HasKey(e => e.Id);
        builder.ConfigureTenantKey();
        
        builder.Property(e => e.WorkflowKey).HasColumnName("workflow_key").HasMaxLength(200).IsRequired();
        builder.Property(e => e.DisplayName).HasColumnName("display_name").HasMaxLength(200).IsRequired();
        builder.Property(e => e.IsDefault).HasColumnName("is_default").IsRequired();

        builder.ConfigureAuditFields();

        builder.HasIndex(e => new { e.TenantId, e.WorkflowKey })
            .IsUnique()
            .HasFilter("deleted_on IS NULL")
            .HasDatabaseName("uq_workflow_definition_tenant_key");
    }
}
