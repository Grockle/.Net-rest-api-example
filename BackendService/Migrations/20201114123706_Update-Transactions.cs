using Microsoft.EntityFrameworkCore.Migrations;

namespace BackendService.Migrations
{
    public partial class UpdateTransactions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "RelatedTransactions");

            migrationBuilder.DropColumn(
                name: "RelatedAmount",
                table: "RelatedTransactions");

            migrationBuilder.AlterColumn<double>(
                name: "Balance",
                table: "GroupBudgetBalances",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "RelatedTransactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "RelatedAmount",
                table: "RelatedTransactions",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<decimal>(
                name: "Balance",
                table: "GroupBudgetBalances",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
