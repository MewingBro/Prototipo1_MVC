using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prototipo1.Migrations
{
    /// <inheritdoc />
    public partial class AddRecintoProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecintoProducto",
                columns: table => new
                {
                    IdRecintoProducto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRecinto = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    Presupuesto = table.Column<double>(type: "float", nullable: false),
                    Desperdicio = table.Column<double>(type: "float", nullable: false),
                    ExistenciasActuales = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecintoProducto", x => x.IdRecintoProducto);
                    table.ForeignKey(
                        name: "FK_RecintoProducto_Producto_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Producto",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecintoProducto_Recinto_IdRecinto",
                        column: x => x.IdRecinto,
                        principalTable: "Recinto",
                        principalColumn: "IdRecinto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecintoProducto_IdProducto",
                table: "RecintoProducto",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_RecintoProducto_IdRecinto",
                table: "RecintoProducto",
                column: "IdRecinto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecintoProducto");
        }
    }
}
