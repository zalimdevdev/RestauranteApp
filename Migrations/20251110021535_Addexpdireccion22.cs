using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestauranteApp.Migrations
{
    /// <inheritdoc />
    public partial class Addexpdireccion22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DireccionNegocio",
                table: "DatosNegocios",
                type: "character varying(55)",
                maxLength: 55,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(15)",
                oldMaxLength: 15,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DireccionNegocio",
                table: "DatosNegocios",
                type: "character varying(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(55)",
                oldMaxLength: 55,
                oldNullable: true);
        }
    }
}
