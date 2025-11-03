using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prototipo1.Migrations
{
    /// <inheritdoc />
    public partial class addUsuarioToCambio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdUsuario",
                table: "Cambio",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Cambio_IdUsuario",
                table: "Cambio",
                column: "IdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Cambio_AspNetUsers_IdUsuario",
                table: "Cambio",
                column: "IdUsuario",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cambio_AspNetUsers_IdUsuario",
                table: "Cambio");

            migrationBuilder.DropIndex(
                name: "IX_Cambio_IdUsuario",
                table: "Cambio");

            migrationBuilder.DropColumn(
                name: "IdUsuario",
                table: "Cambio");
        }
    }
}
