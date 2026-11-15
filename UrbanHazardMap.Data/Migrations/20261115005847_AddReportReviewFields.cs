using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrbanHazardMap.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReportReviewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "IssueReports",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "IssueReports",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "IssueReports");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "IssueReports");
        }
    }
}
