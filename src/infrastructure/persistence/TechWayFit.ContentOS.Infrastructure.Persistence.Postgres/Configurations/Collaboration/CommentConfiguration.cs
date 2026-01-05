using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Collaboration;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Collaboration;

public sealed class CommentConfiguration : IEntityTypeConfiguration<CommentRow>
{
    public void Configure(EntityTypeBuilder<CommentRow> builder)
    {
        builder.ToTable("comment");
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(e => e.EntityInstanceId).HasColumnName("entity_instance_id").IsRequired();
        builder.Property(e => e.ParentCommentId).HasColumnName("parent_comment_id");
        builder.Property(e => e.CommentText).HasColumnName("comment_text").IsRequired();
        builder.Property(e => e.IsInternal).HasColumnName("is_internal").IsRequired();
        builder.Property(e => e.CreatedOn).HasColumnName("created_on").IsRequired();
        builder.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired();
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").IsRequired();

        builder.HasOne(e => e.ParentComment)
            .WithMany()
            .HasForeignKey(e => e.ParentCommentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Creator)
            .WithMany()
            .HasForeignKey(e => e.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.TenantId, e.EntityInstanceId, e.CreatedOn }).HasDatabaseName("ix_comment_entity");
    }
}
