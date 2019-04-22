using Microsoft.EntityFrameworkCore.Migrations;

namespace FillThePool.Core.Data.Migrations
{
    public partial class instructors3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegistrationId",
                table: "Registrations",
                newName: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Registrations",
                newName: "RegistrationId");
        }
    }
}
