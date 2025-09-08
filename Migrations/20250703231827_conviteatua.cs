using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BixWeb.Migrations
{
    /// <inheritdoc />
    public partial class conviteatua : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_Usuario_codUsuario",
                schema: "dbo",
                table: "Eventos");

            migrationBuilder.AddColumn<int>(
                name: "UsuariocodUsuario",
                schema: "dbo",
                table: "Eventos",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "codFilial",
                schema: "dbo",
                table: "Convites",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "codCriador",
                schema: "dbo",
                table: "Convites",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_UsuariocodUsuario",
                schema: "dbo",
                table: "Eventos",
                column: "UsuariocodUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_Usuario_UsuariocodUsuario",
                schema: "dbo",
                table: "Eventos",
                column: "UsuariocodUsuario",
                principalSchema: "dbo",
                principalTable: "Usuario",
                principalColumn: "codUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_Usuario_UsuariocodUsuario",
                schema: "dbo",
                table: "Eventos");

            migrationBuilder.DropIndex(
                name: "IX_Eventos_UsuariocodUsuario",
                schema: "dbo",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "UsuariocodUsuario",
                schema: "dbo",
                table: "Eventos");

            migrationBuilder.AlterColumn<int>(
                name: "codFilial",
                schema: "dbo",
                table: "Convites",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "codCriador",
                schema: "dbo",
                table: "Convites",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
    }
}
