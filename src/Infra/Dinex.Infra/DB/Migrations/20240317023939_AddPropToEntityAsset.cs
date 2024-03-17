using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dinex.Infra.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddPropToEntityAsset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TaxId",
                table: "Assets",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxId",
                table: "Assets");
        }
    }
}
