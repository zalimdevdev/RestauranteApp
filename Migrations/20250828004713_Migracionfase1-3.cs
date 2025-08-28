using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestauranteApp.Migrations
{
    /// <inheritdoc />
    public partial class Migracionfase13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "DatosNegocios",
                columns: new[] { "DatosNegocioId", "DireccionNegocio", "Nombre", "Ruc", "Telefono" },
                values: new object[] { 1, "Tu Dirección", "Nombre de tu Restaurante", "Tu RUC", "Tu Teléfono" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DatosNegocios",
                keyColumn: "DatosNegocioId",
                keyValue: 1);
        }
    }
}
