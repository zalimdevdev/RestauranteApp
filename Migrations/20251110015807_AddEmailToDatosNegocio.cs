using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestauranteApp.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailToDatosNegocio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "DatosNegocios",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "DatosNegocios",
                keyColumn: "DatosNegocioId",
                keyValue: 1,
                column: "Email",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "DatosNegocios");
        }
    }
}
