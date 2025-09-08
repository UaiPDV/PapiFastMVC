using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BixWeb.Migrations
{
    /// <inheritdoc />
    public partial class recibopresente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecibosPresentes",
                schema: "dbo",
                columns: table => new
                {
                    codReciboPresente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nomeCliente = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    cpfCliente = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    emailCliente = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    telefoneCliente = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    codUsuario = table.Column<int>(type: "int", nullable: true),
                    codConvite = table.Column<int>(type: "int", nullable: true),
                    convitecodConvite = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecibosPresentes", x => x.codReciboPresente);
                    table.ForeignKey(
                        name: "FK_RecibosPresentes_Convites_convitecodConvite",
                        column: x => x.convitecodConvite,
                        principalSchema: "dbo",
                        principalTable: "Convites",
                        principalColumn: "codConvite");
                    table.ForeignKey(
                        name: "FK_RecibosPresentes_Usuario_codUsuario",
                        column: x => x.codUsuario,
                        principalSchema: "dbo",
                        principalTable: "Usuario",
                        principalColumn: "codUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PresenteReciboPresente",
                schema: "dbo",
                columns: table => new
                {
                    PresentescodPresente = table.Column<int>(type: "int", nullable: false),
                    listaPresenteReciboscodReciboPresente = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresenteReciboPresente", x => new { x.PresentescodPresente, x.listaPresenteReciboscodReciboPresente });
                    table.ForeignKey(
                        name: "FK_PresenteReciboPresente_Presentes_PresentescodPresente",
                        column: x => x.PresentescodPresente,
                        principalSchema: "dbo",
                        principalTable: "Presentes",
                        principalColumn: "codPresente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PresenteReciboPresente_RecibosPresentes_listaPresenteReciboscodReciboPresente",
                        column: x => x.listaPresenteReciboscodReciboPresente,
                        principalSchema: "dbo",
                        principalTable: "RecibosPresentes",
                        principalColumn: "codReciboPresente",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PresenteReciboPresente_listaPresenteReciboscodReciboPresente",
                schema: "dbo",
                table: "PresenteReciboPresente",
                column: "listaPresenteReciboscodReciboPresente");

            migrationBuilder.CreateIndex(
                name: "IX_RecibosPresentes_codUsuario",
                schema: "dbo",
                table: "RecibosPresentes",
                column: "codUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_RecibosPresentes_convitecodConvite",
                schema: "dbo",
                table: "RecibosPresentes",
                column: "convitecodConvite");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PresenteReciboPresente",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "RecibosPresentes",
                schema: "dbo");
        }
    }
}
