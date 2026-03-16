using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArzotecWebshop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductEan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ean",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ean",
                table: "Products");
        }
    }
}
