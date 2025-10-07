using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Prototipo1.Migrations
{
    /// <inheritdoc />
    public partial class DatosFamilia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Familia",
                columns: new[] { "IdFamilia", "NombreFamilia" },
                values: new object[,]
                {
                    { 1, "BAÑOS" },
                    { 2, "BASES" },
                    { 3, "MADERAS" },
                    { 4, "PAREDES" },
                    { 5, "PAREDES Y PISO" },
                    { 6, "PARQUEO" },
                    { 7, "SEGURIDAD" },
                    { 8, "SOPORTE" },
                    { 9, "SUELOS" },
                    { 10, "VARIOS" },
                    { 11, "Vidrios" },
                    { 12, "PUERTAS " }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Familia",
                keyColumn: "IdFamilia",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Familia",
                keyColumn: "IdFamilia",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Familia",
                keyColumn: "IdFamilia",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Familia",
                keyColumn: "IdFamilia",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Familia",
                keyColumn: "IdFamilia",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Familia",
                keyColumn: "IdFamilia",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Familia",
                keyColumn: "IdFamilia",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Familia",
                keyColumn: "IdFamilia",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Familia",
                keyColumn: "IdFamilia",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Familia",
                keyColumn: "IdFamilia",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Familia",
                keyColumn: "IdFamilia",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Familia",
                keyColumn: "IdFamilia",
                keyValue: 12);
        }
    }
}
