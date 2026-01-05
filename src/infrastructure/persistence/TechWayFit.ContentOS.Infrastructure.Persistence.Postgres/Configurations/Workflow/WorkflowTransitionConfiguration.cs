using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Workflow;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Workflow;

public sealed class WorkflowTransitionConfiguration : IEntityTypeConfiguration<WorkflowTransitionRow>
{
    public void Configure(EntityTypeBuilder<WorkflowTransitionRow> builder)
    {
        builder.ToTable("workflow_transition");
        builder.HasKey(e => e.Id);
        builder.ConfigureTenantKey();
        
        builder.Property(e => e.WorkflowDefinitionId).HasColumnName("workflow_definition_id").IsRequired();
        builder.Property(e => e.FromStateId).HasColumnName("from_state_id").IsRequired();
        builder.Property(e => e.ToStateId).HasColumnName("to_state_id").IsRequired();
        builder.Property(e => e.RequiredAction).HasColumnName("required_action").HasMaxLength(100).IsRequired();

        builder.ConfigureAuditFields();

        builder.HasOne(e => e.WorkflowDefinition)
            .WithMany()
            .HasForeignKey(e => e.WorkflowDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.FromState)
            .WithMany()
            .HasForeignKey(e => e.FromStateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.ToState)
            .WithMany()
            .HasForeignKey(e => e.ToStateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.TenantId, e.WorkflowDefinitionId, e.FromStateId, e.ToStateId })
            .IsUnique()
            .HasFilter("deleted_on IS NULL")
            .HasDatabaseName("uq_workflow_transition_tenant_workflow_states");
    }
}
