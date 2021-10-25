using Microsoft.EntityFrameworkCore.Migrations;

namespace Przychodnia.Migrations
{
    public partial class User_role3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "UserRole");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "UserRole",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
