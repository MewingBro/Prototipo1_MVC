using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Prototipo1.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTipoFactura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TipoFactura",
                columns: table => new
                {
                    IdTipoFactura = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreTipoFactura = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoFactura", x => x.IdTipoFactura);
                });

            migrationBuilder.InsertData(
                table: "TipoFactura",
                columns: new[] { "IdTipoFactura", "NombreTipoFactura" },
                values: new object[,]
                {
                    { 1, "Entrada" },
                    { 2, "Salida" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TipoFactura");
        }
    }
}
