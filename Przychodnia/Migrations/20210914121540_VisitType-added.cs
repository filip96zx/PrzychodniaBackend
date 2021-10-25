using Microsoft.EntityFrameworkCore.Migrations;

namespace Przychodnia.Migrations
{
    public partial class VisitTypeadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VisitType",
                table: "Visits",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VisitType",
                table: "Visits");
        }
    }
}
