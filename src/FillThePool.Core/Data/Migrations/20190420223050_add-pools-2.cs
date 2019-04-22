using Microsoft.EntityFrameworkCore.Migrations;

namespace FillThePool.Core.Data.Migrations
{
    public partial class addpools2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Pools",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "Pools");
        }
    }
}
