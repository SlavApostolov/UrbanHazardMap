using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrbanHazardMap.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixReportOwnerForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IssueReports_AspNetUsers_ReportedByUserId",
                table: "IssueReports");

            migrationBuilder.DropIndex(
                name: "IX_IssueReports_ReportedByUserId",
                table: "IssueReports");

            migrationBuilder.DropColumn(
                name: "ReportedByUserId",
                table: "IssueReports");

            migrationBuilder.CreateIndex(
                name: "IX_IssueReports_ReportedById",
                table: "IssueReports",
                column: "ReportedById");

            migrationBuilder.AddForeignKey(
                name: "FK_IssueReports_AspNetUsers_ReportedById",
                table: "IssueReports",
                column: "ReportedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IssueReports_AspNetUsers_ReportedById",
                table: "IssueReports");

            migrationBuilder.DropIndex(
                name: "IX_IssueReports_ReportedById",
                table: "IssueReports");

            migrationBuilder.AddColumn<string>(
                name: "ReportedByUserId",
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
    }
}
