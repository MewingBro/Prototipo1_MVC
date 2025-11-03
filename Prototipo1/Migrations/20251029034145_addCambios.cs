using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prototipo1.Migrations
{
    /// <inheritdoc />
    public partial class addCambios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cambio",
                columns: table => new
                {
                    IdCambio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdFactura = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cambio", x => x.IdCambio);
                    table.ForeignKey(
                        name: "FK_Cambio_Factura_IdFactura",
                        column: x => x.IdFactura,
                        principalTable: "Factura",
                        principalColumn: "IdFactura",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cambio_IdFactura",
                table: "Cambio",
                column: "IdFactura");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cambio");
        }
    }
}
