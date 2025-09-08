using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BixWeb.Migrations
{
    /// <inheritdoc />
    public partial class recibosupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "codConvite",
                schema: "dbo",
                table: "RecibosPresentes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "dataCriacao",
                schema: "dbo",
                table: "RecibosPresentes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "status",
                schema: "dbo",
                table: "RecibosPresentes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "dataCriacao",
                schema: "dbo",
                table: "RecibosPresentes");

            migrationBuilder.DropColumn(
                name: "status",
                schema: "dbo",
                table: "RecibosPresentes");

            migrationBuilder.AlterColumn<int>(
                name: "codConvite",
                schema: "dbo",
                table: "RecibosPresentes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
