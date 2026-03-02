using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RestauranteApp.Migrations
{
    /// <inheritdoc />
    public partial class refact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facturas_Clientes_ClienteId",
                table: "Facturas");

            migrationBuilder.DropForeignKey(
                name: "FK_Facturas_Clientes_ClienteId1",
                table: "Facturas");

            migrationBuilder.DropForeignKey(
                name: "FK_Facturas_Ordenes_OrdenId",
                table: "Facturas");

            migrationBuilder.DropTable(
                name: "DetallesOrden");

            migrationBuilder.DropTable(
                name: "Ordenes");

            migrationBuilder.DropIndex(
                name: "IX_Facturas_ClienteId1",
                table: "Facturas");

            migrationBuilder.DropIndex(
                name: "IX_Facturas_OrdenId",
                table: "Facturas");

            migrationBuilder.DropColumn(
                name: "Ubicacion",
                table: "Mesas");

            migrationBuilder.DropColumn(
                name: "ClienteId1",
                table: "Facturas");

            migrationBuilder.DropColumn(
                name: "Descuento",
                table: "Facturas");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Facturas");

            migrationBuilder.DropColumn(
                name: "MontoImpuesto",
                table: "Facturas");

            migrationBuilder.DropColumn(
                name: "NumeroFactura",
                table: "Facturas");

            migrationBuilder.DropColumn(
                name: "OrdenId",
                table: "Facturas");

            migrationBuilder.DropColumn(
                name: "Subtotal",
                table: "Facturas");

            migrationBuilder.RenameColumn(
                name: "EstadoReservacion",
                table: "Mesas",
                newName: "Estado");

            migrationBuilder.AddForeignKey(
                name: "FK_Facturas_Clientes_ClienteId",
                table: "Facturas",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "ClienteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facturas_Clientes_ClienteId",
                table: "Facturas");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "Mesas",
                newName: "EstadoReservacion");

            migrationBuilder.AddColumn<string>(
                name: "Ubicacion",
                table: "Mesas",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClienteId1",
                table: "Facturas",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Descuento",
                table: "Facturas",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Facturas",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "MontoImpuesto",
                table: "Facturas",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "NumeroFactura",
                table: "Facturas",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrdenId",
                table: "Facturas",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "Facturas",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Ordenes",
                columns: table => new
                {
                    OrdenId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClienteId = table.Column<int>(type: "integer", nullable: true),
                    EmpleadoId = table.Column<int>(type: "integer", nullable: true),
                    MesaId = table.Column<int>(type: "integer", nullable: false),
                    Descuento = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    FechaHoraCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaHoraModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    MontoImpuesto = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Notas = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    NumeroOrden = table.Column<int>(type: "integer", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ordenes", x => x.OrdenId);
                    table.ForeignKey(
                        name: "FK_Ordenes_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "ClienteId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Ordenes_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "EmpleadoId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Ordenes_Mesas_MesaId",
                        column: x => x.MesaId,
                        principalTable: "Mesas",
                        principalColumn: "MesaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetallesOrden",
                columns: table => new
                {
                    DetalleOrdenId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemMenuId = table.Column<int>(type: "integer", nullable: false),
                    OrdenId = table.Column<int>(type: "integer", nullable: false),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    EstaServido = table.Column<bool>(type: "boolean", nullable: false),
                    FechaHoraServido = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Notas = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PrecioUnitario = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesOrden", x => x.DetalleOrdenId);
                    table.ForeignKey(
                        name: "FK_DetallesOrden_ItemsMenu_ItemMenuId",
                        column: x => x.ItemMenuId,
                        principalTable: "ItemsMenu",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetallesOrden_Ordenes_OrdenId",
                        column: x => x.OrdenId,
                        principalTable: "Ordenes",
                        principalColumn: "OrdenId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_ClienteId1",
                table: "Facturas",
                column: "ClienteId1");

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_OrdenId",
                table: "Facturas",
                column: "OrdenId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetalleOrden_OrdenId",
                table: "DetallesOrden",
                column: "OrdenId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesOrden_ItemMenuId",
                table: "DetallesOrden",
                column: "ItemMenuId");

            migrationBuilder.CreateIndex(
                name: "IX_Orden_Mesa_Estado",
                table: "Ordenes",
                columns: new[] { "MesaId", "Estado" });

            migrationBuilder.CreateIndex(
                name: "IX_Ordenes_ClienteId",
                table: "Ordenes",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Ordenes_EmpleadoId",
                table: "Ordenes",
                column: "EmpleadoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Facturas_Clientes_ClienteId",
                table: "Facturas",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "ClienteId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Facturas_Clientes_ClienteId1",
                table: "Facturas",
                column: "ClienteId1",
                principalTable: "Clientes",
                principalColumn: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Facturas_Ordenes_OrdenId",
                table: "Facturas",
                column: "OrdenId",
                principalTable: "Ordenes",
                principalColumn: "OrdenId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
