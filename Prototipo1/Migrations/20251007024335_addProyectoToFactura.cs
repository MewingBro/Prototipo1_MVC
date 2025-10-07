using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prototipo1.Migrations
{
    /// <inheritdoc />
    public partial class addProyectoToFactura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdProyecto",
                table: "Factura",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Factura_IdProyecto",
                table: "Factura",
                column: "IdProyecto");

            migrationBuilder.AddForeignKey(
                name: "FK_Factura_Proyecto_IdProyecto",
                table: "Factura",
                column: "IdProyecto",
                principalTable: "Proyecto",
                principalColumn: "IdProyecto",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Factura_Proyecto_IdProyecto",
                table: "Factura");

            migrationBuilder.DropIndex(
                name: "IX_Factura_IdProyecto",
                table: "Factura");

            migrationBuilder.DropColumn(
                name: "IdProyecto",
                table: "Factura");
        }
    }
}
