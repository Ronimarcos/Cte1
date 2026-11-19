# Cte1 — Emissor CT-e / MDF-e (Windows Forms, .NET)

## Contexto do produto
Emissor desktop **multiempresa** de CT-e (modelo 57) e MDF-e (modelo 58), modal rodoviário.
Escopo, decisões, modelo de dados e pendências estão em `docs/LEVANTAMENTO.md` — **leia antes de qualquer tarefa** e mantenha-o atualizado quando escopo ou leiaute mudarem.

- Emitente inicial: UF **MT** (`cUF` 51), **Simples Nacional** (`CRT` 1), certificados **A1 e A3**.
- CIOT: informado manualmente no MDF-e na v1 (obrigatório pela NT MDF-e 2026.001, rejeição 684). Geração automática via IPEF é fase futura.
- Averbação automática: fase futura (averbadora ainda não definida).
- Fase 2 (futura): versão web reaproveitando Dominio, Dados e DFe.

## Contexto e Stack
- Linguagem: C# (.NET 10) com Windows Forms.
- Banco: SQLite via EF Core na v1 (PostgreSQL na fase web).
- PDF (DACTE/DAMDFE): QuestPDF.
- Idioma de comentários, logs e mensagens de UI: Português (pt-BR).
- Nomenclatura de código: PascalCase para classes, métodos e propriedades; camelCase para variáveis locais e parâmetros; campos privados com `_campo`.

## Estrutura da solution
```
Cte1.sln
├── src/
│   ├── Emissor.Dominio      → entidades, enums do leiaute, regras de negócio
│   ├── Emissor.Dados        → EF Core (DbContext, migrations, repositórios)
│   ├── Emissor.DFe          → classes dos XSD, montagem, assinatura, validação, webservices
│   └── Emissor.WinForms     → telas; eventos finos chamando serviços
└── tests/
    └── Emissor.Testes
```
Dominio, Dados e DFe **não podem** referenciar Windows Forms.

## Schemas (fonte única do leiaute)
- `schemas/cte/PL_CTe_400_NT2026.005/` — CT-e 4.00 com Reforma Tributária e Simples Nacional (vigente em produção desde 16/11/2026).
- `schemas/mdfe/PL_MDFe_300b_NT2025.001_1.05/` — MDF-e 3.00b.
- `schemas/cte-distdfe/PL_CTeDistDFe_100/` — Distribuição DF-e do CT-e.

Nunca escrever estrutura XML "de memória": gerar classes e validar contra estes XSD.
Não editar os arquivos da pasta `schemas/`; nova versão de leiaute entra em pasta nova.

## Comandos
- Compilar solução: `dotnet build`
- Rodar testes unitários: `dotnet test`
- Formatar código: `dotnet format`

## Regras para Windows Forms
- Não altere arquivos `*.Designer.cs` ou `*.resx` diretamente, a menos que seja explicitamente solicitado.
- Mantenha eventos de controle (`Click`, `Load`, `TextChanged`) finos; delegue regras de negócio e acesso a dados para serviços.
- Nunca bloqueie a UI thread com operações síncronas de I/O ou rede; use `async`/`await`.
- Ao atualizar controles de UI a partir de outras threads, garanta execução segura na thread principal.
- Preserve o padrão de injeção de dependência e o ciclo de vida de formulários/controles já adotados no projeto.

## Regras fiscais e de segurança
- Ambiente padrão de desenvolvimento e testes: **homologação** (`tpAmb` 2). Nunca transmitir para produção sem confirmação explícita do usuário.
- Nunca commitar certificados (`.pfx`, `.p12`), senhas, bancos de dados locais ou XML de documentos reais.
- Certificado A1 e senha armazenados sempre criptografados; A3 acessado pelo repositório do Windows (thumbprint).
- Assinatura atrás da interface `IProvedorCertificado` (implementações A1 e A3).
- Não fixar URLs de webservice de memória: conferir na tabela oficial do portal.
- A busca de documentos na SEFAZ é feita só pelo webservice de Distribuição DF-e; não automatizar portais com CAPTCHA.

## Fluxo de Trabalho (Workflow)
1. Faça perguntas de esclarecimento se os requisitos não estiverem claros.
2. Proponha um plano e liste os arquivos a serem alterados.
3. Implemente a menor mudança viável.
4. Adicione ou atualize testes quando apropriado.
5. Forneça comandos para verificar as mudanças.

Preferências do responsável pelo projeto:
- Entregar blocos de código completos, nunca trechos parciais.
- Ao alterar código existente, mostrar claramente o antes e o depois.
- Referenciar sempre arquivo e local exatos da alteração.
- Mudanças incrementais, preservando versões já validadas.
- Nunca executar ações destrutivas (apagar arquivos, reescrever histórico, `git push --force`) sem confirmação prévia.

## Estado atual
- [x] Levantamento de requisitos v1 (`docs/LEVANTAMENTO.md`).
- [ ] Criar a solution e os cinco projetos vazios, com referências entre eles.
- [ ] Protótipo das telas (principal com seleção de empresa, cadastros, emissão CT-e, emissão MDF-e).
