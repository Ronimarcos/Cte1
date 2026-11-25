using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emissor.Dados.Migracoes
{
    /// <inheritdoc />
    public partial class DocumentosCteMdfe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ctes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: true),
                    TipoServico = table.Column<int>(type: "INTEGER", nullable: true),
                    Cfop = table.Column<string>(type: "TEXT", maxLength: 4, nullable: false),
                    NaturezaOperacao = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    RemetenteId = table.Column<int>(type: "INTEGER", nullable: true),
                    DestinatarioId = table.Column<int>(type: "INTEGER", nullable: true),
                    ExpedidorId = table.Column<int>(type: "INTEGER", nullable: true),
                    RecebedorId = table.Column<int>(type: "INTEGER", nullable: true),
                    Tomador = table.Column<int>(type: "INTEGER", nullable: true),
                    TomadorOutrosId = table.Column<int>(type: "INTEGER", nullable: true),
                    MunicipioInicioCodigo = table.Column<int>(type: "INTEGER", nullable: true),
                    MunicipioFimCodigo = table.Column<int>(type: "INTEGER", nullable: true),
                    ValorCarga = table.Column<decimal>(type: "TEXT", nullable: true),
                    ProdutoPredominante = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    OutrasCaracteristicas = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    ChaveCteComplementado = table.Column<string>(type: "TEXT", maxLength: 44, nullable: false),
                    ValorReceber = table.Column<decimal>(type: "TEXT", nullable: true),
                    Observacoes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AlteradoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EmpresaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Situacao = table.Column<int>(type: "INTEGER", nullable: false),
                    SerieId = table.Column<int>(type: "INTEGER", nullable: true),
                    Numero = table.Column<int>(type: "INTEGER", nullable: true),
                    DataEmissao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ctes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ctes_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ctes_Municipios_MunicipioFimCodigo",
                        column: x => x.MunicipioFimCodigo,
                        principalTable: "Municipios",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ctes_Municipios_MunicipioInicioCodigo",
                        column: x => x.MunicipioInicioCodigo,
                        principalTable: "Municipios",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ctes_Participantes_DestinatarioId",
                        column: x => x.DestinatarioId,
                        principalTable: "Participantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ctes_Participantes_ExpedidorId",
                        column: x => x.ExpedidorId,
                        principalTable: "Participantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ctes_Participantes_RecebedorId",
                        column: x => x.RecebedorId,
                        principalTable: "Participantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ctes_Participantes_RemetenteId",
                        column: x => x.RemetenteId,
                        principalTable: "Participantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ctes_Participantes_TomadorOutrosId",
                        column: x => x.TomadorOutrosId,
                        principalTable: "Participantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ctes_Series_SerieId",
                        column: x => x.SerieId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Mdfes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TipoEmitente = table.Column<int>(type: "INTEGER", nullable: true),
                    TipoTransportador = table.Column<int>(type: "INTEGER", nullable: true),
                    InicioViagem = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UfInicio = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    UfFim = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    Percurso = table.Column<string>(type: "TEXT", nullable: false),
                    MunicipiosCarregamento = table.Column<string>(type: "TEXT", nullable: false),
                    VeiculoTracaoId = table.Column<int>(type: "INTEGER", nullable: true),
                    ReboquesIds = table.Column<string>(type: "TEXT", nullable: false),
                    CondutoresIds = table.Column<string>(type: "TEXT", nullable: false),
                    TipoCarga = table.Column<int>(type: "INTEGER", nullable: true),
                    ProdutoPredominante = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    ApoliceId = table.Column<int>(type: "INTEGER", nullable: true),
                    Averbacoes = table.Column<string>(type: "TEXT", nullable: false),
                    ValorCarga = table.Column<decimal>(type: "TEXT", nullable: true),
                    UnidadePeso = table.Column<int>(type: "INTEGER", nullable: true),
                    PesoCarga = table.Column<decimal>(type: "TEXT", nullable: true),
                    InformacoesComplementares = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AlteradoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EmpresaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Situacao = table.Column<int>(type: "INTEGER", nullable: false),
                    SerieId = table.Column<int>(type: "INTEGER", nullable: true),
                    Numero = table.Column<int>(type: "INTEGER", nullable: true),
                    DataEmissao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mdfes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mdfes_Apolices_ApoliceId",
                        column: x => x.ApoliceId,
                        principalTable: "Apolices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Mdfes_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Mdfes_Series_SerieId",
                        column: x => x.SerieId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Mdfes_Veiculos_VeiculoTracaoId",
                        column: x => x.VeiculoTracaoId,
                        principalTable: "Veiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CteComponentes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    Valor = table.Column<decimal>(type: "TEXT", nullable: false),
                    ConhecimentoTransporteId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CteComponentes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CteComponentes_Ctes_ConhecimentoTransporteId",
                        column: x => x.ConhecimentoTransporteId,
                        principalTable: "Ctes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CteDocumentosAnteriores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CnpjCpfEmitente = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    NomeEmitente = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    ChaveCte = table.Column<string>(type: "TEXT", maxLength: 44, nullable: false),
                    ConhecimentoTransporteId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CteDocumentosAnteriores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CteDocumentosAnteriores_Ctes_ConhecimentoTransporteId",
                        column: x => x.ConhecimentoTransporteId,
                        principalTable: "Ctes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CteNfes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Chave = table.Column<string>(type: "TEXT", maxLength: 44, nullable: false),
                    ConhecimentoTransporteId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CteNfes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CteNfes_Ctes_ConhecimentoTransporteId",
                        column: x => x.ConhecimentoTransporteId,
                        principalTable: "Ctes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CteQuantidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Unidade = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoMedida = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Quantidade = table.Column<decimal>(type: "TEXT", nullable: false),
                    ConhecimentoTransporteId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CteQuantidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CteQuantidades_Ctes_ConhecimentoTransporteId",
                        column: x => x.ConhecimentoTransporteId,
                        principalTable: "Ctes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MdfeCiots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ciot = table.Column<string>(type: "TEXT", maxLength: 12, nullable: false),
                    CpfCnpjResponsavel = table.Column<string>(type: "TEXT", maxLength: 18, nullable: false),
                    ManifestoEletronicoId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MdfeCiots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MdfeCiots_Mdfes_ManifestoEletronicoId",
                        column: x => x.ManifestoEletronicoId,
                        principalTable: "Mdfes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MdfeDescargas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MunicipioCodigo = table.Column<int>(type: "INTEGER", nullable: false),
                    ManifestoEletronicoId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MdfeDescargas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MdfeDescargas_Mdfes_ManifestoEletronicoId",
                        column: x => x.ManifestoEletronicoId,
                        principalTable: "Mdfes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MdfeDescargas_Municipios_MunicipioCodigo",
                        column: x => x.MunicipioCodigo,
                        principalTable: "Municipios",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MdfeDocumentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    Chave = table.Column<string>(type: "TEXT", maxLength: 44, nullable: false),
                    CteId = table.Column<int>(type: "INTEGER", nullable: true),
                    DescargaMdfeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MdfeDocumentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MdfeDocumentos_Ctes_CteId",
                        column: x => x.CteId,
                        principalTable: "Ctes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MdfeDocumentos_MdfeDescargas_DescargaMdfeId",
                        column: x => x.DescargaMdfeId,
                        principalTable: "MdfeDescargas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CteComponentes_ConhecimentoTransporteId",
                table: "CteComponentes",
                column: "ConhecimentoTransporteId");

            migrationBuilder.CreateIndex(
                name: "IX_CteDocumentosAnteriores_ConhecimentoTransporteId",
                table: "CteDocumentosAnteriores",
                column: "ConhecimentoTransporteId");

            migrationBuilder.CreateIndex(
                name: "IX_CteNfes_ConhecimentoTransporteId",
                table: "CteNfes",
                column: "ConhecimentoTransporteId");

            migrationBuilder.CreateIndex(
                name: "IX_CteQuantidades_ConhecimentoTransporteId",
                table: "CteQuantidades",
                column: "ConhecimentoTransporteId");

            migrationBuilder.CreateIndex(
                name: "IX_Ctes_DestinatarioId",
                table: "Ctes",
                column: "DestinatarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Ctes_EmpresaId_DataEmissao",
                table: "Ctes",
                columns: new[] { "EmpresaId", "DataEmissao" });

            migrationBuilder.CreateIndex(
                name: "IX_Ctes_ExpedidorId",
                table: "Ctes",
                column: "ExpedidorId");

            migrationBuilder.CreateIndex(
                name: "IX_Ctes_MunicipioFimCodigo",
                table: "Ctes",
                column: "MunicipioFimCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_Ctes_MunicipioInicioCodigo",
                table: "Ctes",
                column: "MunicipioInicioCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_Ctes_RecebedorId",
                table: "Ctes",
                column: "RecebedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Ctes_RemetenteId",
                table: "Ctes",
                column: "RemetenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Ctes_SerieId",
                table: "Ctes",
                column: "SerieId");

            migrationBuilder.CreateIndex(
                name: "IX_Ctes_TomadorOutrosId",
                table: "Ctes",
                column: "TomadorOutrosId");

            migrationBuilder.CreateIndex(
                name: "IX_MdfeCiots_ManifestoEletronicoId",
                table: "MdfeCiots",
                column: "ManifestoEletronicoId");

            migrationBuilder.CreateIndex(
                name: "IX_MdfeDescargas_ManifestoEletronicoId",
                table: "MdfeDescargas",
                column: "ManifestoEletronicoId");

            migrationBuilder.CreateIndex(
                name: "IX_MdfeDescargas_MunicipioCodigo",
                table: "MdfeDescargas",
                column: "MunicipioCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_MdfeDocumentos_CteId",
                table: "MdfeDocumentos",
                column: "CteId");

            migrationBuilder.CreateIndex(
                name: "IX_MdfeDocumentos_DescargaMdfeId",
                table: "MdfeDocumentos",
                column: "DescargaMdfeId");

            migrationBuilder.CreateIndex(
                name: "IX_Mdfes_ApoliceId",
                table: "Mdfes",
                column: "ApoliceId");

            migrationBuilder.CreateIndex(
                name: "IX_Mdfes_EmpresaId_DataEmissao",
                table: "Mdfes",
                columns: new[] { "EmpresaId", "DataEmissao" });

            migrationBuilder.CreateIndex(
                name: "IX_Mdfes_SerieId",
                table: "Mdfes",
                column: "SerieId");

            migrationBuilder.CreateIndex(
                name: "IX_Mdfes_VeiculoTracaoId",
                table: "Mdfes",
                column: "VeiculoTracaoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CteComponentes");

            migrationBuilder.DropTable(
                name: "CteDocumentosAnteriores");

            migrationBuilder.DropTable(
                name: "CteNfes");

            migrationBuilder.DropTable(
                name: "CteQuantidades");

            migrationBuilder.DropTable(
                name: "MdfeCiots");

            migrationBuilder.DropTable(
                name: "MdfeDocumentos");

            migrationBuilder.DropTable(
                name: "Ctes");

            migrationBuilder.DropTable(
                name: "MdfeDescargas");

            migrationBuilder.DropTable(
                name: "Mdfes");
        }
    }
}
