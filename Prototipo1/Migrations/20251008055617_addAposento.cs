using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prototipo1.Migrations
{
    /// <inheritdoc />
    public partial class addAposento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Aposento",
                columns: table => new
                {
                    IdAposento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreAposento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdNivel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aposento", x => x.IdAposento);
                    table.ForeignKey(
                        name: "FK_Aposento_Nivel_IdNivel",
                        column: x => x.IdNivel,
                        principalTable: "Nivel",
                        principalColumn: "IdNivel",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aposento_IdNivel",
                table: "Aposento",
                column: "IdNivel");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Aposento");
        }
    }
}
