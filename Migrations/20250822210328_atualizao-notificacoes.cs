using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BixWeb.Migrations
{
    /// <inheritdoc />
    public partial class atualizaonotificacoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "exlusivo",
                schema: "dbo",
                table: "Vouchers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "telefone",
                schema: "dbo",
                table: "Filiais",
                type: "nvarchar(14)",
                maxLength: 14,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(18)",
                oldMaxLength: 18,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "cnpj",
                schema: "dbo",
                table: "Filiais",
                type: "nvarchar(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<bool>(
                name: "eventoPrivado",
                schema: "dbo",
                table: "Eventos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "notificar",
                schema: "dbo",
                table: "Convites",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "exlusivo",
                schema: "dbo",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "eventoPrivado",
                schema: "dbo",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "notificar",
                schema: "dbo",
                table: "Convites");

            migrationBuilder.AlterColumn<string>(
                name: "telefone",
                schema: "dbo",
                table: "Filiais",
                type: "nvarchar(18)",
                maxLength: 18,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(14)",
                oldMaxLength: 14,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "cnpj",
                schema: "dbo",
                table: "Filiais",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(18)",
                oldMaxLength: 18);
        }
    }
}
