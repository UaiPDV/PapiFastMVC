using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BixWeb.Migrations
{
    /// <inheritdoc />
    public partial class reciboingresso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "codReciboIngresso",
                schema: "dbo",
                table: "Ingressos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReciboIngressos",
                schema: "dbo",
                columns: table => new
                {
                    codReciboIngresso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nomeCliente = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    cpfCliente = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    dataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    emailCliente = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    telefoneCliente = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    codUsuario = table.Column<int>(type: "int", nullable: true),
                    usuariocodUsuario = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReciboIngressos", x => x.codReciboIngresso);
                    table.ForeignKey(
                        name: "FK_ReciboIngressos_Usuario_usuariocodUsuario",
                        column: x => x.usuariocodUsuario,
                        principalSchema: "dbo",
                        principalTable: "Usuario",
                        principalColumn: "codUsuario");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ingressos_codReciboIngresso",
                schema: "dbo",
                table: "Ingressos",
                column: "codReciboIngresso");

            migrationBuilder.CreateIndex(
                name: "IX_ReciboIngressos_usuariocodUsuario",
                schema: "dbo",
                table: "ReciboIngressos",
                column: "usuariocodUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingressos_ReciboIngressos_codReciboIngresso",
                schema: "dbo",
                table: "Ingressos",
                column: "codReciboIngresso",
                principalSchema: "dbo",
                principalTable: "ReciboIngressos",
                principalColumn: "codReciboIngresso",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingressos_ReciboIngressos_codReciboIngresso",
                schema: "dbo",
                table: "Ingressos");

            migrationBuilder.DropTable(
                name: "ReciboIngressos",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_Ingressos_codReciboIngresso",
                schema: "dbo",
                table: "Ingressos");

            migrationBuilder.DropColumn(
                name: "codReciboIngresso",
                schema: "dbo",
                table: "Ingressos");
        }
    }
}
