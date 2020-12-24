using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BackendService.Migrations
{
    public partial class PersonalAccountTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "Transactions",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Budget",
                table: "Groups",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "GroupCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    UpdateBy = table.Column<int>(nullable: false),
                    UpdateTime = table.Column<DateTime>(nullable: true),
                    GroupId = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersonalAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    UpdateBy = table.Column<int>(nullable: false),
                    UpdateTime = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    CategoryName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersonalCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    UpdateBy = table.Column<int>(nullable: false),
                    UpdateTime = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersonalTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    UpdateBy = table.Column<int>(nullable: false),
                    UpdateTime = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    TransactionTime = table.Column<DateTime>(nullable: false),
                    AccountId = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    Category = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalTransactions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupCategories");

            migrationBuilder.DropTable(
                name: "PersonalAccounts");

            migrationBuilder.DropTable(
                name: "PersonalCategories");

            migrationBuilder.DropTable(
                name: "PersonalTransactions");

            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Budget",
                table: "Groups");
        }
    }
}
