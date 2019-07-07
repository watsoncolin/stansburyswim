using Microsoft.EntityFrameworkCore.Migrations;

namespace FillThePool.Core.Data.Migrations
{
    public partial class addwaitlist2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowedPurchase",
                table: "Waitlist",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Waitlist_ProfileId",
                table: "Waitlist",
                column: "ProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Waitlist_Profiles_ProfileId",
                table: "Waitlist",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Waitlist_Profiles_ProfileId",
                table: "Waitlist");

            migrationBuilder.DropIndex(
                name: "IX_Waitlist_ProfileId",
                table: "Waitlist");

            migrationBuilder.DropColumn(
                name: "AllowedPurchase",
                table: "Waitlist");
        }
    }
}
