using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RestauranteApp.Migrations
{
    /// <inheritdoc />
    public partial class AddMovimientoStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemMenuIngredientes_Ingredientes_IngredienteId",
                table: "ItemMenuIngredientes");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaStockInicial",
                table: "Ingredientes",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "StockInicial",
                table: "Ingredientes",
                type: "numeric(18,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "MovimientosStock",
                columns: table => new
                {
                    MovimientoId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IngredienteId = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FacturaId = table.Column<int>(type: "integer", nullable: true),
                    DetalleFacturaId = table.Column<int>(type: "integer", nullable: true),
                    Observacion = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosStock", x => x.MovimientoId);
                    table.ForeignKey(
                        name: "FK_MovimientosStock_DetalleFacturas_DetalleFacturaId",
                        column: x => x.DetalleFacturaId,
                        principalTable: "DetalleFacturas",
                        principalColumn: "DetalleFacturaId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MovimientosStock_Facturas_FacturaId",
                        column: x => x.FacturaId,
                        principalTable: "Facturas",
                        principalColumn: "FacturaId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MovimientosStock_Ingredientes_IngredienteId",
                        column: x => x.IngredienteId,
                        principalTable: "Ingredientes",
                        principalColumn: "IngredienteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosStock_DetalleFacturaId",
                table: "MovimientosStock",
                column: "DetalleFacturaId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosStock_FacturaId",
                table: "MovimientosStock",
                column: "FacturaId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosStock_IngredienteId",
                table: "MovimientosStock",
                column: "IngredienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemMenuIngredientes_Ingredientes_IngredienteId",
                table: "ItemMenuIngredientes",
                column: "IngredienteId",
                principalTable: "Ingredientes",
                principalColumn: "IngredienteId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemMenuIngredientes_Ingredientes_IngredienteId",
                table: "ItemMenuIngredientes");

            migrationBuilder.DropTable(
                name: "MovimientosStock");

            migrationBuilder.DropColumn(
                name: "FechaStockInicial",
                table: "Ingredientes");

            migrationBuilder.DropColumn(
                name: "StockInicial",
                table: "Ingredientes");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemMenuIngredientes_Ingredientes_IngredienteId",
                table: "ItemMenuIngredientes",
                column: "IngredienteId",
                principalTable: "Ingredientes",
                principalColumn: "IngredienteId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
