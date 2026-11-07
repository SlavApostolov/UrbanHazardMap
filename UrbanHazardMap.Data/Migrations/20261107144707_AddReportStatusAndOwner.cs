using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrbanHazardMap.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReportStatusAndOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReportedBy",
                table: "IssueReports",
                newName: "ReportedByUserId");

            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "IssueReports",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReportedById",
                table: "IssueReports",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IssueReports_ReportedByUserId",
                table: "IssueReports",
                column: "ReportedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_IssueReports_AspNetUsers_ReportedByUserId",
                table: "IssueReports",
                column: "ReportedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IssueReports_AspNetUsers_ReportedByUserId",
                table: "IssueReports");

            migrationBuilder.DropIndex(
                name: "IX_IssueReports_ReportedByUserId",
                table: "IssueReports");

            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "IssueReports");

            migrationBuilder.DropColumn(
                name: "ReportedById",
                table: "IssueReports");

            migrationBuilder.RenameColumn(
                name: "ReportedByUserId",
                table: "IssueReports",
                newName: "ReportedBy");
        }
    }
}
