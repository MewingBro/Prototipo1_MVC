using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prototipo1.Migrations
{
    /// <inheritdoc />
    public partial class AddRecinto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Recinto",
                columns: table => new
                {
                    IdRecinto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreRecinto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdAposento = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recinto", x => x.IdRecinto);
                    table.ForeignKey(
                        name: "FK_Recinto_Aposento_IdAposento",
                        column: x => x.IdAposento,
                        principalTable: "Aposento",
                        principalColumn: "IdAposento",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recinto_IdAposento",
                table: "Recinto",
                column: "IdAposento");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Recinto");
        }
    }
}
