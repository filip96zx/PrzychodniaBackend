using Microsoft.EntityFrameworkCore.Migrations;

namespace Przychodnia.Migrations
{
    public partial class User_Roles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_Users_UserId1",
                table: "UserRole");

            migrationBuilder.DropIndex(
                name: "IX_UserRole_UserId1",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserRole");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
