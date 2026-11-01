using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrbanHazardMap.Migrations
{
    /// <inheritdoc />
    public partial class AddVolunteerEventIssueReportRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IssueReportId",
                table: "VolunteerEvents",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerEvents_IssueReportId",
                table: "VolunteerEvents",
                column: "IssueReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_VolunteerEvents_IssueReports_IssueReportId",
                table: "VolunteerEvents",
                column: "IssueReportId",
                principalTable: "IssueReports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VolunteerEvents_IssueReports_IssueReportId",
                table: "VolunteerEvents");

            migrationBuilder.DropIndex(
                name: "IX_VolunteerEvents_IssueReportId",
                table: "VolunteerEvents");

            migrationBuilder.DropColumn(
                name: "IssueReportId",
                table: "VolunteerEvents");
        }
    }
}
