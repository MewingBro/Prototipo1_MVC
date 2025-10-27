using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prototipo1.Migrations
{
    /// <inheritdoc />
    public partial class addnulls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Factura_Recinto_IdRecinto",
                table: "Factura");

            migrationBuilder.AlterColumn<int>(
                name: "IdRecinto",
                table: "Factura",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Factura_Recinto_IdRecinto",
                table: "Factura",
                column: "IdRecinto",
                principalTable: "Recinto",
                principalColumn: "IdRecinto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Factura_Recinto_IdRecinto",
                table: "Factura");

            migrationBuilder.AlterColumn<int>(
                name: "IdRecinto",
                table: "Factura",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Factura_Recinto_IdRecinto",
                table: "Factura",
                column: "IdRecinto",
                principalTable: "Recinto",
                principalColumn: "IdRecinto",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
