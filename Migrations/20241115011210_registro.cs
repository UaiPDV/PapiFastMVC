using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BixWeb.Migrations
{
    /// <inheritdoc />
    public partial class registro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LoginAPI",
                schema: "dbo",
                table: "LoginAPI");

            migrationBuilder.AddColumn<int>(
                name: "codRegistro",
                schema: "dbo",
                table: "UsuarioFiliais",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "client_id",
                schema: "dbo",
                table: "LoginAPI",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "codLoginAPI",
                schema: "dbo",
                table: "LoginAPI",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LoginAPI",
                schema: "dbo",
                table: "LoginAPI",
                column: "codLoginAPI");

            migrationBuilder.CreateTable(
                name: "Dispositivos",
                schema: "dbo",
                columns: table => new
                {
                    codDipositivo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    enderecoMac = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ativo = table.Column<bool>(type: "bit", nullable: false),
                    dispositivo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tipoDisposito = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    codFilial = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dispositivos", x => x.codDipositivo);
                    table.ForeignKey(
                        name: "FK_Dispositivos_Filiais_codFilial",
                        column: x => x.codFilial,
                        principalSchema: "dbo",
                        principalTable: "Filiais",
                        principalColumn: "codFilial",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenerativeIAs",
                schema: "dbo",
                columns: table => new
                {
                    codIA = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tipoIA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    chaveIA = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenerativeIAs", x => x.codIA);
                });

            migrationBuilder.CreateTable(
                name: "RegistroLogins",
                schema: "dbo",
                columns: table => new
                {
                    codRegistro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    inicioLogin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fimLogin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ativo = table.Column<bool>(type: "bit", nullable: false),
                    codDispositivo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistroLogins", x => x.codRegistro);
                    table.ForeignKey(
                        name: "FK_RegistroLogins_Dispositivos_codDispositivo",
                        column: x => x.codDispositivo,
                        principalSchema: "dbo",
                        principalTable: "Dispositivos",
                        principalColumn: "codDipositivo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioFiliais_codRegistro",
                schema: "dbo",
                table: "UsuarioFiliais",
                column: "codRegistro",
                unique: true,
                filter: "[codRegistro] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Dispositivos_codFilial",
                schema: "dbo",
                table: "Dispositivos",
                column: "codFilial");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroLogins_codDispositivo",
                schema: "dbo",
                table: "RegistroLogins",
                column: "codDispositivo");

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioFiliais_RegistroLogins_codRegistro",
                schema: "dbo",
                table: "UsuarioFiliais",
                column: "codRegistro",
                principalSchema: "dbo",
                principalTable: "RegistroLogins",
                principalColumn: "codRegistro");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioFiliais_RegistroLogins_codRegistro",
                schema: "dbo",
                table: "UsuarioFiliais");

            migrationBuilder.DropTable(
                name: "GenerativeIAs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "RegistroLogins",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Dispositivos",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_UsuarioFiliais_codRegistro",
                schema: "dbo",
                table: "UsuarioFiliais");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LoginAPI",
                schema: "dbo",
                table: "LoginAPI");

            migrationBuilder.DropColumn(
                name: "codRegistro",
                schema: "dbo",
                table: "UsuarioFiliais");

            migrationBuilder.DropColumn(
                name: "codLoginAPI",
                schema: "dbo",
                table: "LoginAPI");

            migrationBuilder.AlterColumn<string>(
                name: "client_id",
                schema: "dbo",
                table: "LoginAPI",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LoginAPI",
                schema: "dbo",
                table: "LoginAPI",
                column: "client_id");
        }
    }
}
