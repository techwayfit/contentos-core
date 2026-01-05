using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Workflow;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Workflow;

public sealed class WorkflowStateConfiguration : IEntityTypeConfiguration<WorkflowStateRow>
{
    public void Configure(EntityTypeBuilder<WorkflowStateRow> builder)
    {
        builder.ToTable("workflow_state");
        builder.HasKey(e => e.Id);
        builder.ConfigureTenantKey();
        
        builder.Property(e => e.WorkflowDefinitionId).HasColumnName("workflow_definition_id").IsRequired();
        builder.Property(e => e.StateKey).HasColumnName("state_key").HasMaxLength(100).IsRequired();
        builder.Property(e => e.DisplayName).HasColumnName("display_name").HasMaxLength(200).IsRequired();
        builder.Property(e => e.IsTerminal).HasColumnName("is_terminal").IsRequired();

        builder.ConfigureAuditFields();

        builder.HasOne(e => e.WorkflowDefinition)
            .WithMany()
            .HasForeignKey(e => e.WorkflowDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.TenantId, e.WorkflowDefinitionId, e.StateKey })
            .IsUnique()
            .HasFilter("deleted_on IS NULL")
            .HasDatabaseName("uq_workflow_state_tenant_workflow_key");
    }
}
