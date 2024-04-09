using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RzumeAPI.Migrations
{
    public partial class UpdateFileTableDTO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFile_AspNetUsers_UserId",
                table: "UserFile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserFile",
                table: "UserFile");

            migrationBuilder.RenameTable(
                name: "UserFile",
                newName: "Userfile");

            migrationBuilder.RenameIndex(
                name: "IX_UserFile_UserId",
                table: "Userfile",
                newName: "IX_Userfile_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Userfile",
                table: "Userfile",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Userfile_AspNetUsers_UserId",
                table: "Userfile",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Userfile_AspNetUsers_UserId",
                table: "Userfile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Userfile",
                table: "Userfile");

            migrationBuilder.RenameTable(
                name: "Userfile",
                newName: "UserFile");

            migrationBuilder.RenameIndex(
                name: "IX_Userfile_UserId",
                table: "UserFile",
                newName: "IX_UserFile_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserFile",
                table: "UserFile",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFile_AspNetUsers_UserId",
                table: "UserFile",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
