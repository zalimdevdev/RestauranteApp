using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RestauranteApp.Migrations
{
    /// <inheritdoc />
    public partial class preing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredientes_Proveedores_ProveedorId",
                table: "Ingredientes");

            migrationBuilder.DropIndex(
                name: "IX_Ingredientes_ProveedorId",
                table: "Ingredientes");

            migrationBuilder.DropColumn(
                name: "ProveedorId",
                table: "Ingredientes");

            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "Ingredientes",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Costo",
                table: "Ingredientes",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "ItemMenuIngredientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemMenuId = table.Column<int>(type: "integer", nullable: false),
                    IngredienteId = table.Column<int>(type: "integer", nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric(18,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemMenuIngredientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemMenuIngredientes_Ingredientes_IngredienteId",
                        column: x => x.IngredienteId,
                        principalTable: "Ingredientes",
                        principalColumn: "IngredienteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemMenuIngredientes_ItemsMenu_ItemMenuId",
                        column: x => x.ItemMenuId,
                        principalTable: "ItemsMenu",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemMenuIngredientes_IngredienteId",
                table: "ItemMenuIngredientes",
                column: "IngredienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemMenuIngredientes_ItemMenuId",
                table: "ItemMenuIngredientes",
                column: "ItemMenuId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemMenuIngredientes");

            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Ingredientes");

            migrationBuilder.DropColumn(
                name: "Costo",
                table: "Ingredientes");

            migrationBuilder.AddColumn<int>(
                name: "ProveedorId",
                table: "Ingredientes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ingredientes_ProveedorId",
                table: "Ingredientes",
                column: "ProveedorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredientes_Proveedores_ProveedorId",
                table: "Ingredientes",
                column: "ProveedorId",
                principalTable: "Proveedores",
                principalColumn: "ProveedorId");
        }
    }
}
