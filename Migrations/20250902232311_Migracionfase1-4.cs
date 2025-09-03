using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestauranteApp.Migrations
{
    /// <inheritdoc />
    public partial class Migracionfase14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MesaId1",
                table: "Mesas",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MesaId",
                table: "Facturas",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mesas_MesaId1",
                table: "Mesas",
                column: "MesaId1");

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_MesaId",
                table: "Facturas",
                column: "MesaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Facturas_Mesas_MesaId",
                table: "Facturas",
                column: "MesaId",
                principalTable: "Mesas",
                principalColumn: "MesaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mesas_Mesas_MesaId1",
                table: "Mesas",
                column: "MesaId1",
                principalTable: "Mesas",
                principalColumn: "MesaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facturas_Mesas_MesaId",
                table: "Facturas");

            migrationBuilder.DropForeignKey(
                name: "FK_Mesas_Mesas_MesaId1",
                table: "Mesas");

            migrationBuilder.DropIndex(
                name: "IX_Mesas_MesaId1",
                table: "Mesas");

            migrationBuilder.DropIndex(
                name: "IX_Facturas_MesaId",
                table: "Facturas");

            migrationBuilder.DropColumn(
                name: "MesaId1",
                table: "Mesas");

            migrationBuilder.DropColumn(
                name: "MesaId",
                table: "Facturas");
        }
    }
}
