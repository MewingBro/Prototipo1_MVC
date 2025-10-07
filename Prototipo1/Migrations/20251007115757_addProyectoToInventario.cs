using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prototipo1.Migrations
{
    /// <inheritdoc />
    public partial class addProyectoToInventario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdProyecto",
                table: "Inventario",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Inventario_IdProyecto",
                table: "Inventario",
                column: "IdProyecto");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventario_Proyecto_IdProyecto",
                table: "Inventario",
                column: "IdProyecto",
                principalTable: "Proyecto",
                principalColumn: "IdProyecto",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventario_Proyecto_IdProyecto",
                table: "Inventario");

            migrationBuilder.DropIndex(
                name: "IX_Inventario_IdProyecto",
                table: "Inventario");

            migrationBuilder.DropColumn(
                name: "IdProyecto",
                table: "Inventario");
        }
    }
}
