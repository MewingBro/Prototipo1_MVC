using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prototipo1.Migrations
{
    /// <inheritdoc />
    public partial class CambioDetalle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comentario",
                table: "Cambio",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "CambioDetalle",
                columns: table => new
                {
                    IdCambioDetalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCambio = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    Presupuesto = table.Column<double>(type: "float", nullable: false),
                    ExistenciasActuales = table.Column<double>(type: "float", nullable: false),
                    CantidadDisminuida = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CambioDetalle", x => x.IdCambioDetalle);
                    table.ForeignKey(
                        name: "FK_CambioDetalle_Cambio_IdCambio",
                        column: x => x.IdCambio,
                        principalTable: "Cambio",
                        principalColumn: "IdCambio",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CambioDetalle_Producto_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Producto",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CambioDetalle_IdCambio",
                table: "CambioDetalle",
                column: "IdCambio");

            migrationBuilder.CreateIndex(
                name: "IX_CambioDetalle_IdProducto",
                table: "CambioDetalle",
                column: "IdProducto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CambioDetalle");

            migrationBuilder.DropColumn(
                name: "Comentario",
                table: "Cambio");
        }
    }
}
