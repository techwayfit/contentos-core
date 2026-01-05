using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Processes;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Processes;

public sealed class ProcessInstanceConfiguration : IEntityTypeConfiguration<ProcessInstanceRow>
{
    public void Configure(EntityTypeBuilder<ProcessInstanceRow> builder)
    {
        builder.ToTable("process_instance");
        builder.HasKey(e => e.Id);
        builder.ConfigureTenantKey();
        
        builder.Property(e => e.ProcessDefinitionId).HasColumnName("process_definition_id").IsRequired();
        builder.Property(e => e.EntityInstanceId).HasColumnName("entity_instance_id").IsRequired();
        builder.Property(e => e.CurrentState).HasColumnName("current_state").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
        builder.Property(e => e.StartedOn).HasColumnName("started_on").IsRequired();
        builder.Property(e => e.DueOn).HasColumnName("due_on");
        builder.Property(e => e.CompletedOn).HasColumnName("completed_on");
        builder.Property(e => e.ContextDataJson).HasColumnName("context_data_json").HasColumnType("jsonb").IsRequired();

        builder.ConfigureAuditFields();

        builder.HasOne(e => e.ProcessDefinition)
            .WithMany()
            .HasForeignKey(e => e.ProcessDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.TenantId, e.Status }).HasDatabaseName("ix_process_instance_tenant_status");
        builder.HasIndex(e => new { e.TenantId, e.ProcessDefinitionId }).HasDatabaseName("ix_process_instance_tenant_definition");
    }
}
