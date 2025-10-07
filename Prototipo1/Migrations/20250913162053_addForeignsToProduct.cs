using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prototipo1.Migrations
{
    /// <inheritdoc />
    public partial class addForeignsToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdFamilia",
                table: "Producto",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdUnidad",
                table: "Producto",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Producto_IdFamilia",
                table: "Producto",
                column: "IdFamilia");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_IdUnidad",
                table: "Producto",
                column: "IdUnidad");

            migrationBuilder.AddForeignKey(
                name: "FK_Producto_Familia_IdFamilia",
                table: "Producto",
                column: "IdFamilia",
                principalTable: "Familia",
                principalColumn: "IdFamilia",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Producto_Unidad_IdUnidad",
                table: "Producto",
                column: "IdUnidad",
                principalTable: "Unidad",
                principalColumn: "IdUnidad",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Producto_Familia_IdFamilia",
                table: "Producto");

            migrationBuilder.DropForeignKey(
                name: "FK_Producto_Unidad_IdUnidad",
                table: "Producto");

            migrationBuilder.DropIndex(
                name: "IX_Producto_IdFamilia",
                table: "Producto");

            migrationBuilder.DropIndex(
                name: "IX_Producto_IdUnidad",
                table: "Producto");

            migrationBuilder.DropColumn(
                name: "IdFamilia",
                table: "Producto");

            migrationBuilder.DropColumn(
                name: "IdUnidad",
                table: "Producto");
        }
    }
}
