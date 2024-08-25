using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RzumeAPI.Migrations
{
    public partial class AddsRelationShipBtwCmpanyAndApplication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyID",
                table: "Application",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Application_CompanyID",
                table: "Application",
                column: "CompanyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Application_Company_CompanyID",
                table: "Application",
                column: "CompanyID",
                principalTable: "Company",
                principalColumn: "CompanyID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Application_Company_CompanyID",
                table: "Application");

            migrationBuilder.DropIndex(
                name: "IX_Application_CompanyID",
                table: "Application");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                table: "Application");
        }
    }
}
