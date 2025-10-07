using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prototipo1.Migrations
{
    /// <inheritdoc />
    public partial class AddNivel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CodigoProducto",
                table: "Producto",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Nivel",
                columns: table => new
                {
                    IdNivel = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreNivel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdProyecto = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nivel", x => x.IdNivel);
                    table.ForeignKey(
                        name: "FK_Nivel_Proyecto_IdProyecto",
                        column: x => x.IdProyecto,
                        principalTable: "Proyecto",
                        principalColumn: "IdProyecto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Producto_CodigoProducto",
                table: "Producto",
                column: "CodigoProducto",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Nivel_IdProyecto",
                table: "Nivel",
                column: "IdProyecto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Nivel");

            migrationBuilder.DropIndex(
                name: "IX_Producto_CodigoProducto",
                table: "Producto");

            migrationBuilder.AlterColumn<string>(
                name: "CodigoProducto",
                table: "Producto",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
