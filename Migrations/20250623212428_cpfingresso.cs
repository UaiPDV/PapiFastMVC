using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BixWeb.Migrations
{
    /// <inheritdoc />
    public partial class cpfingresso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "cpfIngresso",
                schema: "dbo",
                table: "Ingressos",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "codUsuario",
                schema: "dbo",
                table: "Eventos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "codFilial",
                schema: "dbo",
                table: "Eventos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_Usuario_codUsuario",
                schema: "dbo",
                table: "Eventos",
                column: "codUsuario",
                principalSchema: "dbo",
                principalTable: "Usuario",
                principalColumn: "codUsuario",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_Usuario_codUsuario",
                schema: "dbo",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "cpfIngresso",
                schema: "dbo",
                table: "Ingressos");

            migrationBuilder.AlterColumn<int>(
                name: "codUsuario",
                schema: "dbo",
                table: "Eventos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "codFilial",
                schema: "dbo",
                table: "Eventos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
