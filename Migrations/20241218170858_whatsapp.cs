using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BixWeb.Migrations
{
    /// <inheritdoc />
    public partial class whatsapp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WhatsApps",
                schema: "dbo",
                columns: table => new
                {
                    codWhats = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sessao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ntelefone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    qrCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    criado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    atualizado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    codFilial = table.Column<int>(type: "int", nullable: true),
                    codUsuario = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhatsApps", x => x.codWhats);
                    table.ForeignKey(
                        name: "FK_WhatsApps_Filiais_codFilial",
                        column: x => x.codFilial,
                        principalSchema: "dbo",
                        principalTable: "Filiais",
                        principalColumn: "codFilial");
                    table.ForeignKey(
                        name: "FK_WhatsApps_Usuario_codUsuario",
                        column: x => x.codUsuario,
                        principalSchema: "dbo",
                        principalTable: "Usuario",
                        principalColumn: "codUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WhatsApps_codFilial",
                schema: "dbo",
                table: "WhatsApps",
                column: "codFilial",
                unique: true,
                filter: "[codFilial] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_WhatsApps_codUsuario",
                schema: "dbo",
                table: "WhatsApps",
                column: "codUsuario",
                unique: true,
                filter: "[codUsuario] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WhatsApps",
                schema: "dbo");
        }
    }
}
