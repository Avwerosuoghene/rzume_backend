using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RzumeAPI.Migrations
{
    public partial class Adjusts_Education_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "YearOfGraudation",
                table: "Education",
                newName: "GraduationDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GraduationDate",
                table: "Education",
                newName: "YearOfGraudation");
        }
    }
}
