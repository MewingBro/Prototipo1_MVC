using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prototipo1.Migrations
{
    /// <inheritdoc />
    public partial class AgregarFacturaProductos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FacturaProducto",
                columns: table => new
                {
                    IdFacturaProducto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdFactura = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    CantidadAumentada = table.Column<int>(type: "int", nullable: false),
                    EntregadoA = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacturaProducto", x => x.IdFacturaProducto);
                    table.ForeignKey(
                        name: "FK_FacturaProducto_Factura_IdFactura",
                        column: x => x.IdFactura,
                        principalTable: "Factura",
                        principalColumn: "IdFactura",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacturaProducto_Producto_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Producto",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FacturaProducto_IdFactura",
                table: "FacturaProducto",
                column: "IdFactura");

            migrationBuilder.CreateIndex(
                name: "IX_FacturaProducto_IdProducto",
                table: "FacturaProducto",
                column: "IdProducto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FacturaProducto");
        }
    }
}
