using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BixWeb.Migrations
{
    /// <inheritdoc />
    public partial class relacaoconvitereciboPre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecibosPresentes_Convites_convitecodConvite",
                schema: "dbo",
                table: "RecibosPresentes");

            migrationBuilder.DropIndex(
                name: "IX_RecibosPresentes_convitecodConvite",
                schema: "dbo",
                table: "RecibosPresentes");

            migrationBuilder.DropColumn(
                name: "convitecodConvite",
                schema: "dbo",
                table: "RecibosPresentes");

            migrationBuilder.CreateIndex(
                name: "IX_RecibosPresentes_codConvite",
                schema: "dbo",
                table: "RecibosPresentes",
                column: "codConvite");

            migrationBuilder.AddForeignKey(
                name: "FK_RecibosPresentes_Convites_codConvite",
                schema: "dbo",
                table: "RecibosPresentes",
                column: "codConvite",
                principalSchema: "dbo",
                principalTable: "Convites",
                principalColumn: "codConvite",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecibosPresentes_Convites_codConvite",
                schema: "dbo",
                table: "RecibosPresentes");

            migrationBuilder.DropIndex(
                name: "IX_RecibosPresentes_codConvite",
                schema: "dbo",
                table: "RecibosPresentes");

            migrationBuilder.AddColumn<int>(
                name: "convitecodConvite",
                schema: "dbo",
                table: "RecibosPresentes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecibosPresentes_convitecodConvite",
                schema: "dbo",
                table: "RecibosPresentes",
                column: "convitecodConvite");

            migrationBuilder.AddForeignKey(
                name: "FK_RecibosPresentes_Convites_convitecodConvite",
                schema: "dbo",
                table: "RecibosPresentes",
                column: "convitecodConvite",
                principalSchema: "dbo",
                principalTable: "Convites",
                principalColumn: "codConvite");
        }
    }
}
