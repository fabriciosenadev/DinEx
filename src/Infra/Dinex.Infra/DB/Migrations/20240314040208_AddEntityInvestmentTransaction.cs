using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dinex.Infra.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddEntityInvestmentTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvestmentTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TransactionHistoryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Applicable = table.Column<int>(type: "INTEGER", nullable: false),
                    TransactionType = table.Column<int>(type: "INTEGER", nullable: false),
                    AssetId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AssetUnitPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    AssetTransactionAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    AssetQuantity = table.Column<int>(type: "INTEGER", nullable: false),
                    StockBrokerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentTransactions", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvestmentTransactions");
        }
    }
}
