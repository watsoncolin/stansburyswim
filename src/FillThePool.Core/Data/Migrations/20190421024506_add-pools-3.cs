using Microsoft.EntityFrameworkCore.Migrations;

namespace FillThePool.Core.Data.Migrations
{
    public partial class addpools3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Details",
                table: "Pools",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Details",
                table: "Pools");
        }
    }
}
