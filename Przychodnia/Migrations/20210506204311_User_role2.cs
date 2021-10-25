using Microsoft.EntityFrameworkCore.Migrations;

namespace Przychodnia.Migrations
{
    public partial class User_role2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "UserRole",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "UserRole",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_UserId1",
                table: "UserRole",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_Users_UserId1",
                table: "UserRole",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_Users_UserId1",
                table: "UserRole");

            migrationBuilder.DropIndex(
                name: "IX_UserRole_UserId1",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserRole");
        }
    }
}
