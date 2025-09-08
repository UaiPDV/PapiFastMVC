using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BixWeb.Migrations
{
    /// <inheritdoc />
    public partial class presentes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ListaPresentes",
                schema: "dbo",
                columns: table => new
                {
                    codListaPres = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    codEvento = table.Column<int>(type: "int", nullable: false),
                    codUsuario = table.Column<int>(type: "int", nullable: false),
                    dataLista = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListaPresentes", x => x.codListaPres);
                    table.ForeignKey(
                        name: "FK_ListaPresentes_Eventos_codEvento",
                        column: x => x.codEvento,
                        principalSchema: "dbo",
                        principalTable: "Eventos",
                        principalColumn: "codEvento",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ListaPresentes_Usuario_codUsuario",
                        column: x => x.codUsuario,
                        principalSchema: "dbo",
                        principalTable: "Usuario",
                        principalColumn: "codUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Presentes",
                schema: "dbo",
                columns: table => new
                {
                    codPresente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Preco = table.Column<double>(type: "float", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Imagem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    codListaPresente = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presentes", x => x.codPresente);
                    table.ForeignKey(
                        name: "FK_Presentes_ListaPresentes_codListaPresente",
                        column: x => x.codListaPresente,
                        principalSchema: "dbo",
                        principalTable: "ListaPresentes",
                        principalColumn: "codListaPres",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ListaPresentes_codEvento",
                schema: "dbo",
                table: "ListaPresentes",
                column: "codEvento");

            migrationBuilder.CreateIndex(
                name: "IX_ListaPresentes_codUsuario",
                schema: "dbo",
                table: "ListaPresentes",
                column: "codUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Presentes_codListaPresente",
                schema: "dbo",
                table: "Presentes",
                column: "codListaPresente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Presentes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ListaPresentes",
                schema: "dbo");
        }
    }
}
