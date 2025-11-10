using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestauranteApp.Migrations
{
    /// <inheritdoc />
    public partial class AddImagenUrlToItemMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facturas_Pedidos_PedidoId",
                table: "Facturas");

            migrationBuilder.DropIndex(
                name: "IX_Facturas_PedidoId",
                table: "Facturas");

            migrationBuilder.DropColumn(
                name: "PedidoId",
                table: "Facturas");

            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "ItemsMenu",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "ItemsMenu");

            migrationBuilder.AddColumn<int>(
                name: "PedidoId",
                table: "Facturas",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_PedidoId",
                table: "Facturas",
                column: "PedidoId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Facturas_Pedidos_PedidoId",
                table: "Facturas",
                column: "PedidoId",
                principalTable: "Pedidos",
                principalColumn: "PedidoId");
        }
    }
}
