using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RzumeAPI.Migrations
{
    public partial class updatesSkillnUserModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skill_AspNetUsers_UserId",
                table: "Skill");

            migrationBuilder.DropIndex(
                name: "IX_Skill_UserId",
                table: "Skill");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Skill");

            migrationBuilder.CreateTable(
                name: "SkillUser",
                columns: table => new
                {
                    SkillsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillUser", x => new { x.SkillsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_SkillUser_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SkillUser_Skill_SkillsId",
                        column: x => x.SkillsId,
                        principalTable: "Skill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SkillUser_UsersId",
                table: "SkillUser",
                column: "UsersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SkillUser");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Skill",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Skill_UserId",
                table: "Skill",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Skill_AspNetUsers_UserId",
                table: "Skill",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
