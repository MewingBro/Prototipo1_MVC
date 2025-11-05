using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prototipo1.Migrations
{
    /// <inheritdoc />
    public partial class CorregidaMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MediaArtimetica",
                table: "Producto");

            migrationBuilder.AddColumn<int>(
                name: "MediaAritmetica",
                table: "Producto",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MediaAritmetica",
                table: "Producto");

            migrationBuilder.AddColumn<string>(
                name: "MediaArtimetica",
                table: "Producto",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
