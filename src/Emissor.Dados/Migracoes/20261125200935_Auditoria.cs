using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emissor.Dados.Migracoes
{
    /// <inheritdoc />
    public partial class Auditoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Auditoria",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DataHora = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UsuarioId = table.Column<int>(type: "INTEGER", nullable: true),
                    UsuarioLogin = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    EmpresaId = table.Column<int>(type: "INTEGER", nullable: true),
                    Acao = table.Column<int>(type: "INTEGER", nullable: false),
                    Entidade = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    RegistroId = table.Column<int>(type: "INTEGER", nullable: true),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auditoria", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Auditoria_DataHora",
                table: "Auditoria",
                column: "DataHora");

            migrationBuilder.CreateIndex(
                name: "IX_Auditoria_EmpresaId_DataHora",
                table: "Auditoria",
                columns: new[] { "EmpresaId", "DataHora" });

            migrationBuilder.CreateIndex(
                name: "IX_Auditoria_UsuarioId_DataHora",
                table: "Auditoria",
                columns: new[] { "UsuarioId", "DataHora" });

            // A auditoria só recebe inclusões: o próprio banco recusa alteração e exclusão das linhas.
            migrationBuilder.Sql(@"CREATE TRIGGER ""TR_Auditoria_SemAlteracao"" BEFORE UPDATE ON ""Auditoria""
BEGIN
    SELECT RAISE(ABORT, 'A auditoria não pode ser alterada.');
END;");
            migrationBuilder.Sql(@"CREATE TRIGGER ""TR_Auditoria_SemExclusao"" BEFORE DELETE ON ""Auditoria""
BEGIN
    SELECT RAISE(ABORT, 'A auditoria não pode ser excluída.');
END;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS ""TR_Auditoria_SemExclusao"";");
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS ""TR_Auditoria_SemAlteracao"";");

            migrationBuilder.DropTable(
                name: "Auditoria");
        }
    }
}
