using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prototipo1.Migrations
{
    /// <inheritdoc />
    public partial class addSalidas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FacturaSalidaProducto",
                columns: table => new
                {
                    IdFacturaSalidaProducto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdFactura = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    IdRecinto = table.Column<int>(type: "int", nullable: false),
                    CantidadDisminuida = table.Column<int>(type: "int", nullable: false),
                    EntregadoA = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacturaSalidaProducto", x => x.IdFacturaSalidaProducto);
                    table.ForeignKey(
                        name: "FK_FacturaSalidaProducto_Factura_IdFactura",
                        column: x => x.IdFactura,
                        principalTable: "Factura",
                        principalColumn: "IdFactura",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacturaSalidaProducto_Producto_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Producto",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Cascade);

                    table.ForeignKey(
    name: "FK_FacturaSalidaProducto_Recinto_IdRecinto",
    column: x => x.IdRecinto,
    principalTable: "Recinto",
    principalColumn: "IdRecinto",
    onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FacturaSalidaProducto_IdFactura",
                table: "FacturaSalidaProducto",
                column: "IdFactura");

            migrationBuilder.CreateIndex(
                name: "IX_FacturaSalidaProducto_IdProducto",
                table: "FacturaSalidaProducto",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_FacturaSalidaProducto_IdRecinto",
                table: "FacturaSalidaProducto",
                column: "IdRecinto");

        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FacturaSalidaProducto");
        }
    }
}
