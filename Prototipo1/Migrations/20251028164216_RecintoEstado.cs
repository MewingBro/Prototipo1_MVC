using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prototipo1.Migrations
{
    /// <inheritdoc />
    public partial class RecintoEstado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstadoRecinto",
                table: "Recinto",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstadoRecinto",
                table: "Recinto");
        }
    }
}
