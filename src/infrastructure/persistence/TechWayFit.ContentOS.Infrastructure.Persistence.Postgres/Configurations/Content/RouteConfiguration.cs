using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Content;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Content;

public sealed class RouteConfiguration : IEntityTypeConfiguration<RouteRow>
{
    public void Configure(EntityTypeBuilder<RouteRow> builder)
    {
        builder.ToTable("route");
        builder.HasKey(e => e.Id);
        builder.ConfigureTenantSiteKeys();
        
        builder.Property(e => e.NodeId).HasColumnName("node_id").IsRequired();
        builder.Property(e => e.RoutePath).HasColumnName("route_path").HasMaxLength(2000).IsRequired();
        builder.Property(e => e.IsPrimary).HasColumnName("is_primary").IsRequired();

        builder.ConfigureAuditFields();

        builder.HasOne(e => e.Node)
            .WithMany()
            .HasForeignKey(e => e.NodeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.TenantId, e.SiteId, e.RoutePath })
            .IsUnique()
            .HasFilter("deleted_on IS NULL")
            .HasDatabaseName("uq_route_tenant_site_path");
    }
}
