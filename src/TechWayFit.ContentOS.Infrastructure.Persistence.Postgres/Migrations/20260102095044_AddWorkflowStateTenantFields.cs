using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkflowStateTenantFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_workflow_states_content_item",
                table: "workflow_states");

            migrationBuilder.AddColumn<string>(
                name: "environment",
                table: "workflow_states",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "site_id",
                table: "workflow_states",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "tenant_id",
                table: "workflow_states",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_workflow_states_content_item",
                table: "workflow_states",
                columns: new[] { "tenant_id", "content_item_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_workflow_states_tenant",
                table: "workflow_states",
                columns: new[] { "tenant_id", "site_id", "environment" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_workflow_states_content_item",
                table: "workflow_states");

            migrationBuilder.DropIndex(
                name: "ix_workflow_states_tenant",
                table: "workflow_states");

            migrationBuilder.DropColumn(
                name: "environment",
                table: "workflow_states");

            migrationBuilder.DropColumn(
                name: "site_id",
                table: "workflow_states");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                table: "workflow_states");

            migrationBuilder.CreateIndex(
                name: "ix_workflow_states_content_item",
                table: "workflow_states",
                column: "content_item_id",
                unique: true);
        }
    }
}
