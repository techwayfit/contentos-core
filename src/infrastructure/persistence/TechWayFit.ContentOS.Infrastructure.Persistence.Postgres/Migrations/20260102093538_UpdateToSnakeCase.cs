using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class UpdateToSnakeCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_content_localizations_content_items_ContentItemId",
                table: "content_localizations");

            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "workflow_states",
                newName: "comment");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "workflow_states",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "TransitionedBy",
                table: "workflow_states",
                newName: "transitioned_by");

            migrationBuilder.RenameColumn(
                name: "TransitionedAt",
                table: "workflow_states",
                newName: "transitioned_at");

            migrationBuilder.RenameColumn(
                name: "PreviousStatus",
                table: "workflow_states",
                newName: "previous_status");

            migrationBuilder.RenameColumn(
                name: "CurrentStatus",
                table: "workflow_states",
                newName: "current_status");

            migrationBuilder.RenameColumn(
                name: "ContentItemId",
                table: "workflow_states",
                newName: "content_item_id");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "content_localizations",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Slug",
                table: "content_localizations",
                newName: "slug");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "content_localizations",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "content_localizations",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "LanguageCode",
                table: "content_localizations",
                newName: "language_code");

            migrationBuilder.RenameColumn(
                name: "FieldsJson",
                table: "content_localizations",
                newName: "fields_json");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "content_localizations",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "ContentItemId",
                table: "content_localizations",
                newName: "content_item_id");

            migrationBuilder.RenameColumn(
                name: "Environment",
                table: "content_items",
                newName: "environment");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "content_items",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "WorkflowStatus",
                table: "content_items",
                newName: "workflow_status");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "content_items",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "content_items",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                table: "content_items",
                newName: "tenant_id");

            migrationBuilder.RenameColumn(
                name: "SiteId",
                table: "content_items",
                newName: "site_id");

            migrationBuilder.RenameColumn(
                name: "DefaultLanguage",
                table: "content_items",
                newName: "default_language");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "content_items",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "content_items",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "ContentType",
                table: "content_items",
                newName: "content_type");

            migrationBuilder.AddForeignKey(
                name: "FK_content_localizations_content_items_content_item_id",
                table: "content_localizations",
                column: "content_item_id",
                principalTable: "content_items",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_content_localizations_content_items_content_item_id",
                table: "content_localizations");

            migrationBuilder.RenameColumn(
                name: "comment",
                table: "workflow_states",
                newName: "Comment");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "workflow_states",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "transitioned_by",
                table: "workflow_states",
                newName: "TransitionedBy");

            migrationBuilder.RenameColumn(
                name: "transitioned_at",
                table: "workflow_states",
                newName: "TransitionedAt");

            migrationBuilder.RenameColumn(
                name: "previous_status",
                table: "workflow_states",
                newName: "PreviousStatus");

            migrationBuilder.RenameColumn(
                name: "current_status",
                table: "workflow_states",
                newName: "CurrentStatus");

            migrationBuilder.RenameColumn(
                name: "content_item_id",
                table: "workflow_states",
                newName: "ContentItemId");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "content_localizations",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "slug",
                table: "content_localizations",
                newName: "Slug");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "content_localizations",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "content_localizations",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "language_code",
                table: "content_localizations",
                newName: "LanguageCode");

            migrationBuilder.RenameColumn(
                name: "fields_json",
                table: "content_localizations",
                newName: "FieldsJson");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "content_localizations",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "content_item_id",
                table: "content_localizations",
                newName: "ContentItemId");

            migrationBuilder.RenameColumn(
                name: "environment",
                table: "content_items",
                newName: "Environment");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "content_items",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "workflow_status",
                table: "content_items",
                newName: "WorkflowStatus");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "content_items",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "content_items",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "tenant_id",
                table: "content_items",
                newName: "TenantId");

            migrationBuilder.RenameColumn(
                name: "site_id",
                table: "content_items",
                newName: "SiteId");

            migrationBuilder.RenameColumn(
                name: "default_language",
                table: "content_items",
                newName: "DefaultLanguage");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "content_items",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "content_items",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "content_type",
                table: "content_items",
                newName: "ContentType");

            migrationBuilder.AddForeignKey(
                name: "FK_content_localizations_content_items_ContentItemId",
                table: "content_localizations",
                column: "ContentItemId",
                principalTable: "content_items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
