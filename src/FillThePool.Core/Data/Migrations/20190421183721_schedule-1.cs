using Microsoft.EntityFrameworkCore.Migrations;

namespace FillThePool.Core.Data.Migrations
{
    public partial class schedule1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TransactionId",
                table: "Registrations",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_TransactionId",
                table: "Registrations",
                column: "TransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_Transactions_TransactionId",
                table: "Registrations",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "TransactionId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_Transactions_TransactionId",
                table: "Registrations");

            migrationBuilder.DropIndex(
                name: "IX_Registrations_TransactionId",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Registrations");
        }
    }
}
