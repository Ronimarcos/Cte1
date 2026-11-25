# Gera as classes C# do leiaute a partir dos XSD da pasta schemas/ (os XSD não são editados).
# Uso, na raiz da solution:  pwsh ferramentas/gerar-classes-dfe.ps1
# Ferramenta: dotnet-xscgen (XmlSchemaClassGenerator), registrada em dotnet-tools.json.
# O código gerado é commitado; não editar à mão: rode o script de novo quando o leiaute mudar.

$ErrorActionPreference = 'Stop'
$raiz = Split-Path -Parent $PSScriptRoot

dotnet tool restore | Out-Null

function Gerar([string]$pastaSchemas, [string]$saidaRelativa, [string]$namespaceXml, [string]$namespaceCs, [string[]]$arquivos) {
    $saida = Join-Path $raiz $saidaRelativa
    New-Item -ItemType Directory -Force $saida | Out-Null
    Get-ChildItem $saida -Filter *.cs | Remove-Item
    Push-Location (Join-Path $raiz $pastaSchemas)
    try {
        # Caminho de saída relativo, para não gravar caminhos da máquina no cabeçalho do código gerado.
        $relativa = [System.IO.Path]::GetRelativePath((Get-Location).Path, $saida)
        dotnet xscgen -o $relativa `
            -n "$namespaceXml=$namespaceCs" `
            -n "http://www.w3.org/2000/09/xmldsig#=Emissor.DFe.Leiaute.Assinatura" `
            --nc --nr -0 @arquivos
        if ($LASTEXITCODE -ne 0) { throw "xscgen falhou em $pastaSchemas" }
    }
    finally {
        Pop-Location
    }
}

# CT-e 4.00 (NT 2026.005): documento e modal rodoviário (o modal vai em infModal como XML à parte).
Gerar 'schemas/cte/PL_CTe_400_NT2026.005' 'src/Emissor.DFe/Leiaute/Cte' `
    'http://www.portalfiscal.inf.br/cte' 'Emissor.DFe.Leiaute.Cte' `
    @('cte_v4.00.xsd', 'cteModalRodoviario_v4.00.xsd')

Write-Host 'Classes geradas.'
