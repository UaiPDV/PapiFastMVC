using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BixWeb.Migrations
{
    /// <inheritdoc />
    public partial class alteracaocodconvite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListaPresentes_Eventos_codEvento",
                schema: "dbo",
                table: "ListaPresentes");

            migrationBuilder.DropIndex(
                name: "IX_ListaPresentes_codEvento",
                schema: "dbo",
                table: "ListaPresentes");

            migrationBuilder.RenameColumn(
                name: "codEvento",
                schema: "dbo",
                table: "ListaPresentes",
                newName: "codConvite");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                schema: "dbo",
                table: "Presentes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Imagem",
                schema: "dbo",
                table: "Presentes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                schema: "dbo",
                table: "Presentes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ListaPresentes_codConvite",
                schema: "dbo",
                table: "ListaPresentes",
                column: "codConvite",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ListaPresentes_Convites_codConvite",
                schema: "dbo",
                table: "ListaPresentes",
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
                name: "FK_ListaPresentes_Convites_codConvite",
                schema: "dbo",
                table: "ListaPresentes");

            migrationBuilder.DropIndex(
                name: "IX_ListaPresentes_codConvite",
                schema: "dbo",
                table: "ListaPresentes");

            migrationBuilder.RenameColumn(
                name: "codConvite",
                schema: "dbo",
                table: "ListaPresentes",
                newName: "codEvento");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                schema: "dbo",
                table: "Presentes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Imagem",
                schema: "dbo",
                table: "Presentes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                schema: "dbo",
                table: "Presentes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ListaPresentes_codEvento",
                schema: "dbo",
                table: "ListaPresentes",
                column: "codEvento");

            migrationBuilder.AddForeignKey(
                name: "FK_ListaPresentes_Eventos_codEvento",
                schema: "dbo",
                table: "ListaPresentes",
                column: "codEvento",
                principalSchema: "dbo",
                principalTable: "Eventos",
                principalColumn: "codEvento",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
