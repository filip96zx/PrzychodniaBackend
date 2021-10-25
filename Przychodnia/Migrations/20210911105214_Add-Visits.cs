using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Przychodnia.Migrations
{
    public partial class AddVisits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PESEL",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "Visits",
                columns: table => new
                {
                    VisitId = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    PatientId = table.Column<int>(type: "integer", nullable: false),
                    VisitStatus = table.Column<int>(type: "integer", nullable: false),
                    Findings = table.Column<List<string>>(type: "text[]", nullable: true),
                    Prescriptions = table.Column<List<string>>(type: "text[]", nullable: true),
                    Messages = table.Column<List<string>>(type: "text[]", nullable: true),
                    UserId1 = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visits", x => new { x.VisitId, x.UserId });
                    table.ForeignKey(
                        name: "FK_Visits_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Visits_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Visits_UserId",
                table: "Visits",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_UserId1",
                table: "Visits",
                column: "UserId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Visits");

            migrationBuilder.AddColumn<string>(
                name: "PESEL",
                table: "Users",
                type: "text",
                nullable: true);
        }
    }
}
