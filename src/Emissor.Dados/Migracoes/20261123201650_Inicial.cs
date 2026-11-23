using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emissor.Dados.Migracoes
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Municipios",
                columns: table => new
                {
                    Codigo = table.Column<int>(type: "INTEGER", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Uf = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Municipios", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RazaoSocial = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    NomeFantasia = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Cnpj = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    Cpf = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false),
                    Ie = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    IeSt = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    Crt = table.Column<int>(type: "INTEGER", nullable: true),
                    RegimeIbsCbs = table.Column<int>(type: "INTEGER", nullable: true),
                    Rntrc = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false),
                    Logradouro = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Numero = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Complemento = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Bairro = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    MunicipioCodigo = table.Column<int>(type: "INTEGER", nullable: true),
                    Cep = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false),
                    Fone = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    AmbientePadrao = table.Column<int>(type: "INTEGER", nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AlteradoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Empresas_Municipios_MunicipioCodigo",
                        column: x => x.MunicipioCodigo,
                        principalTable: "Municipios",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Apolices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Responsavel = table.Column<int>(type: "INTEGER", nullable: false),
                    CnpjCpfResponsavel = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    NomeSeguradora = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    CnpjSeguradora = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    NumeroApolice = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AlteradoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EmpresaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apolices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Apolices_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Certificados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: true),
                    ArquivoPfxCriptografado = table.Column<byte[]>(type: "BLOB", nullable: true),
                    SenhaCriptografada = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Thumbprint = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    Titular = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    CnpjTitular = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    ValidoDe = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ValidoAte = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AlteradoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EmpresaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certificados_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Motoristas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Cpf = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AlteradoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EmpresaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Motoristas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Motoristas_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Participantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CnpjCpf = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    IndicadorIe = table.Column<int>(type: "INTEGER", nullable: true),
                    Ie = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    RazaoSocial = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    NomeFantasia = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Logradouro = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Numero = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Complemento = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Bairro = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    MunicipioCodigo = table.Column<int>(type: "INTEGER", nullable: true),
                    Cep = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false),
                    Fone = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AlteradoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EmpresaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Participantes_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Participantes_Municipios_MunicipioCodigo",
                        column: x => x.MunicipioCodigo,
                        principalTable: "Municipios",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProprietariosVeiculo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CnpjCpf = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    Rntrc = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    TipoProprietario = table.Column<int>(type: "INTEGER", nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AlteradoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EmpresaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProprietariosVeiculo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProprietariosVeiculo_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Series",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Modelo = table.Column<int>(type: "INTEGER", nullable: false),
                    Serie = table.Column<int>(type: "INTEGER", nullable: false),
                    Ambiente = table.Column<int>(type: "INTEGER", nullable: false),
                    ProximoNumero = table.Column<int>(type: "INTEGER", nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AlteradoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EmpresaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Series", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Series_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Veiculos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    CodigoInterno = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Placa = table.Column<string>(type: "TEXT", maxLength: 7, nullable: false),
                    Renavam = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false),
                    TaraKg = table.Column<int>(type: "INTEGER", nullable: false),
                    CapacidadeKg = table.Column<int>(type: "INTEGER", nullable: true),
                    CapacidadeM3 = table.Column<int>(type: "INTEGER", nullable: true),
                    TipoRodado = table.Column<int>(type: "INTEGER", nullable: true),
                    TipoCarroceria = table.Column<int>(type: "INTEGER", nullable: false),
                    Uf = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    Proprio = table.Column<bool>(type: "INTEGER", nullable: false),
                    ProprietarioId = table.Column<int>(type: "INTEGER", nullable: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AlteradoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EmpresaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veiculos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Veiculos_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Veiculos_ProprietariosVeiculo_ProprietarioId",
                        column: x => x.ProprietarioId,
                        principalTable: "ProprietariosVeiculo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Apolices_EmpresaId_CnpjSeguradora_NumeroApolice",
                table: "Apolices",
                columns: new[] { "EmpresaId", "CnpjSeguradora", "NumeroApolice" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Certificados_EmpresaId_Thumbprint",
                table: "Certificados",
                columns: new[] { "EmpresaId", "Thumbprint" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_Cnpj",
                table: "Empresas",
                column: "Cnpj",
                unique: true,
                filter: "\"Cnpj\" <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_Cpf",
                table: "Empresas",
                column: "Cpf",
                unique: true,
                filter: "\"Cpf\" <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_MunicipioCodigo",
                table: "Empresas",
                column: "MunicipioCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_Motoristas_EmpresaId_Cpf",
                table: "Motoristas",
                columns: new[] { "EmpresaId", "Cpf" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Municipios_Uf_Nome",
                table: "Municipios",
                columns: new[] { "Uf", "Nome" });

            migrationBuilder.CreateIndex(
                name: "IX_Participantes_EmpresaId_CnpjCpf",
                table: "Participantes",
                columns: new[] { "EmpresaId", "CnpjCpf" });

            migrationBuilder.CreateIndex(
                name: "IX_Participantes_MunicipioCodigo",
                table: "Participantes",
                column: "MunicipioCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_ProprietariosVeiculo_EmpresaId_CnpjCpf",
                table: "ProprietariosVeiculo",
                columns: new[] { "EmpresaId", "CnpjCpf" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Series_EmpresaId_Modelo_Serie_Ambiente",
                table: "Series",
                columns: new[] { "EmpresaId", "Modelo", "Serie", "Ambiente" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Veiculos_EmpresaId_Placa",
                table: "Veiculos",
                columns: new[] { "EmpresaId", "Placa" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Veiculos_ProprietarioId",
                table: "Veiculos",
                column: "ProprietarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Apolices");

            migrationBuilder.DropTable(
                name: "Certificados");

            migrationBuilder.DropTable(
                name: "Motoristas");

            migrationBuilder.DropTable(
                name: "Participantes");

            migrationBuilder.DropTable(
                name: "Series");

            migrationBuilder.DropTable(
                name: "Veiculos");

            migrationBuilder.DropTable(
                name: "ProprietariosVeiculo");

            migrationBuilder.DropTable(
                name: "Empresas");

            migrationBuilder.DropTable(
                name: "Municipios");
        }
    }
}
