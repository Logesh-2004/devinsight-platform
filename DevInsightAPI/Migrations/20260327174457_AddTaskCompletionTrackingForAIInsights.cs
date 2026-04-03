using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevInsightAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskCompletionTrackingForAIInsights : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "Tasks",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Tasks");
        }
    }
}
