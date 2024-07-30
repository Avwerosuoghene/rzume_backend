using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RzumeAPI.Migrations
{
    public partial class updatesFavoritesModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationID",
                table: "Favorites");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationID",
                table: "Favorites",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
