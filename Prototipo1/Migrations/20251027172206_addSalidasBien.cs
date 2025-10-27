using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prototipo1.Migrations
{
    /// <inheritdoc />
    public partial class addSalidasBien : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacturaSalidaProducto_Recinto_IdRecinto",
                table: "FacturaSalidaProducto");

            migrationBuilder.DropIndex(
                name: "IX_FacturaSalidaProducto_IdRecinto",
                table: "FacturaSalidaProducto");

            migrationBuilder.DropColumn(
                name: "IdRecinto",
                table: "FacturaSalidaProducto");

            migrationBuilder.AddColumn<int>(
                name: "IdRecinto",
                table: "Factura",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Factura_IdRecinto",
                table: "Factura",
                column: "IdRecinto");

            migrationBuilder.AddForeignKey(
                name: "FK_Factura_Recinto_IdRecinto",
                table: "Factura",
                column: "IdRecinto",
                principalTable: "Recinto",
                principalColumn: "IdRecinto",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Factura_Recinto_IdRecinto",
                table: "Factura");

            migrationBuilder.DropIndex(
                name: "IX_Factura_IdRecinto",
                table: "Factura");

            migrationBuilder.DropColumn(
                name: "IdRecinto",
                table: "Factura");

            migrationBuilder.AddColumn<int>(
                name: "IdRecinto",
                table: "FacturaSalidaProducto",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FacturaSalidaProducto_IdRecinto",
                table: "FacturaSalidaProducto",
                column: "IdRecinto");

            migrationBuilder.AddForeignKey(
                name: "FK_FacturaSalidaProducto_Recinto_IdRecinto",
                table: "FacturaSalidaProducto",
                column: "IdRecinto",
                principalTable: "Recinto",
                principalColumn: "IdRecinto",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
