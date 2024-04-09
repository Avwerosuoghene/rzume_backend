using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RzumeAPI.Migrations
{
    public partial class UpdateFileTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileType",
                table: "UserFile",
                newName: "FileCategory");

            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "UserFile",
                newName: "FileBytes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileCategory",
                table: "UserFile",
                newName: "FileType");

            migrationBuilder.RenameColumn(
                name: "FileBytes",
                table: "UserFile",
                newName: "FilePath");
        }
    }
}
