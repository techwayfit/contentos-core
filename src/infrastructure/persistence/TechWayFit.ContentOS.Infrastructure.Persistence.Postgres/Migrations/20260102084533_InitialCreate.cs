using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "content_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    SiteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Environment = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DefaultLanguage = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    WorkflowStatus = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_content_items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workflow_states",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentStatus = table.Column<int>(type: "integer", nullable: false),
                    PreviousStatus = table.Column<int>(type: "integer", nullable: true),
                    TransitionedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    TransitionedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workflow_states", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "content_localizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Slug = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FieldsJson = table.Column<string>(type: "jsonb", nullable: false, defaultValue: "{}"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_content_localizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_content_localizations_content_items_ContentItemId",
                        column: x => x.ContentItemId,
                        principalTable: "content_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_content_items_content_type",
                table: "content_items",
                column: "ContentType");

            migrationBuilder.CreateIndex(
                name: "ix_content_items_created_at",
                table: "content_items",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "ix_content_items_tenant",
                table: "content_items",
                columns: new[] { "TenantId", "SiteId", "Environment" });

            migrationBuilder.CreateIndex(
                name: "ix_content_items_workflow_status",
                table: "content_items",
                column: "WorkflowStatus");

            migrationBuilder.CreateIndex(
                name: "ix_content_localizations_language",
                table: "content_localizations",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "ix_content_localizations_slug",
                table: "content_localizations",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "ix_content_localizations_unique",
                table: "content_localizations",
                columns: new[] { "ContentItemId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_workflow_states_content_item",
                table: "workflow_states",
                column: "ContentItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_workflow_states_current_status",
                table: "workflow_states",
                column: "CurrentStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "content_localizations");

            migrationBuilder.DropTable(
                name: "workflow_states");

            migrationBuilder.DropTable(
                name: "content_items");
        }
    }
}
