using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emissor.Dados.Migracoes
{
    /// <inheritdoc />
    public partial class Eventos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AutorizadoEm",
                table: "Mdfes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Chave",
                table: "Mdfes",
                type: "TEXT",
                maxLength: 44,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Protocolo",
                table: "Mdfes",
                type: "TEXT",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "AutorizadoEm",
                table: "Ctes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Chave",
                table: "Ctes",
                type: "TEXT",
                maxLength: 44,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Protocolo",
                table: "Ctes",
                type: "TEXT",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Eventos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    DocumentoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Sequencia = table.Column<int>(type: "INTEGER", nullable: false),
                    DataHora = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Justificativa = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    DataEncerramento = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MunicipioEncerramentoCodigo = table.Column<int>(type: "INTEGER", nullable: true),
                    NomeCondutor = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    CpfCondutor = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false),
                    Situacao = table.Column<int>(type: "INTEGER", nullable: false),
                    Xml = table.Column<string>(type: "TEXT", nullable: false),
                    Protocolo = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    CodigoStatus = table.Column<int>(type: "INTEGER", nullable: true),
                    Motivo = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    RegistradoEm = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AlteradoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EmpresaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eventos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Eventos_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Eventos_Municipios_MunicipioEncerramentoCodigo",
                        column: x => x.MunicipioEncerramentoCodigo,
                        principalTable: "Municipios",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventoCorrecoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Grupo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Campo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Valor = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    NumeroItem = table.Column<int>(type: "INTEGER", nullable: true),
                    EventoDocumentoId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoCorrecoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventoCorrecoes_Eventos_EventoDocumentoId",
                        column: x => x.EventoDocumentoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventoCorrecoes_EventoDocumentoId",
                table: "EventoCorrecoes",
                column: "EventoDocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_EmpresaId_Tipo_DocumentoId_Sequencia",
                table: "Eventos",
                columns: new[] { "EmpresaId", "Tipo", "DocumentoId", "Sequencia" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_MunicipioEncerramentoCodigo",
                table: "Eventos",
                column: "MunicipioEncerramentoCodigo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventoCorrecoes");

            migrationBuilder.DropTable(
                name: "Eventos");

            migrationBuilder.DropColumn(
                name: "AutorizadoEm",
                table: "Mdfes");

            migrationBuilder.DropColumn(
                name: "Chave",
                table: "Mdfes");

            migrationBuilder.DropColumn(
                name: "Protocolo",
                table: "Mdfes");

            migrationBuilder.DropColumn(
                name: "AutorizadoEm",
                table: "Ctes");

            migrationBuilder.DropColumn(
                name: "Chave",
                table: "Ctes");

            migrationBuilder.DropColumn(
                name: "Protocolo",
                table: "Ctes");
        }
    }
}
