using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emissor.Dados.Migracoes
{
    /// <inheritdoc />
    public partial class ProprietarioIeUf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ie",
                table: "ProprietariosVeiculo",
                type: "TEXT",
                maxLength: 14,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Uf",
                table: "ProprietariosVeiculo",
                type: "TEXT",
                maxLength: 2,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ie",
                table: "ProprietariosVeiculo");

            migrationBuilder.DropColumn(
                name: "Uf",
                table: "ProprietariosVeiculo");
        }
    }
}
