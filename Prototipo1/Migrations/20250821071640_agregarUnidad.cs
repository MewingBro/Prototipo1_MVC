using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Prototipo1.Migrations
{
    /// <inheritdoc />
    public partial class agregarUnidad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NombreFamilia",
                table: "Familia",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Unidad",
                columns: table => new
                {
                    IdUnidad = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreUnidad = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unidad", x => x.IdUnidad);
                });

            migrationBuilder.InsertData(
                table: "Unidad",
                columns: new[] { "IdUnidad", "NombreUnidad" },
                values: new object[,]
                {
                    { 1, "BOLSA" },
                    { 2, "CUBETA" },
                    { 3, "M²" },
                    { 4, "PIEZA" },
                    { 5, "UNIDAD" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Unidad");

            migrationBuilder.AlterColumn<string>(
                name: "NombreFamilia",
                table: "Familia",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);
        }
    }
}
