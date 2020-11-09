using Microsoft.EntityFrameworkCore.Migrations;

namespace BackendService.Migrations
{
    public partial class TransactionsUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TransactionId",
                table: "RelatedTransactions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MoneyType",
                table: "Groups",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "RelatedTransactions");

            migrationBuilder.DropColumn(
                name: "MoneyType",
                table: "Groups");
        }
    }
}
