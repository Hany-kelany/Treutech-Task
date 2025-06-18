using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task_TrueTech.Migrations
{
    /// <inheritdoc />
    public partial class addingQuantityCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalQuantity",
                table: "items",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalQuantity",
                table: "items");
        }
    }
}
