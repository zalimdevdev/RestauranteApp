using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestauranteApp.Migrations
{
    /// <inheritdoc />
    public partial class AddLogoUrlToDatosNegocio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "DatosNegocios",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "DatosNegocios",
                keyColumn: "DatosNegocioId",
                keyValue: 1,
                column: "LogoUrl",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "DatosNegocios");
        }
    }
}
