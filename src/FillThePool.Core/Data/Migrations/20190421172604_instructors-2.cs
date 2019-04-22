using Microsoft.EntityFrameworkCore.Migrations;

namespace FillThePool.Core.Data.Migrations
{
    public partial class instructors2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InstructorId",
                table: "Schedules",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_InstructorId",
                table: "Schedules",
                column: "InstructorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Instructors_InstructorId",
                table: "Schedules",
                column: "InstructorId",
                principalTable: "Instructors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Instructors_InstructorId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_InstructorId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "InstructorId",
                table: "Schedules");
        }
    }
}
