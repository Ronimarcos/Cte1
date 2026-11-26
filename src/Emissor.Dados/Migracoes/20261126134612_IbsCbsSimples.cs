using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emissor.Dados.Migracoes
{
    /// <inheritdoc />
    public partial class IbsCbsSimples : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AliquotaCbsSimples",
                table: "Empresas",
                type: "TEXT",
                precision: 7,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AliquotaIbsSimples",
                table: "Empresas",
                type: "TEXT",
                precision: 7,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassTribIbsCbs",
                table: "Empresas",
                type: "TEXT",
                maxLength: 6,
                nullable: false,
                defaultValue: "000001");

            migrationBuilder.AddColumn<string>(
                name: "CstIbsCbs",
                table: "Empresas",
                type: "TEXT",
                maxLength: 3,
                nullable: false,
                defaultValue: "000");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AliquotaCbsSimples",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "AliquotaIbsSimples",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "ClassTribIbsCbs",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "CstIbsCbs",
                table: "Empresas");
        }
    }
}
