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
- Responder ao usuário sempre em pt-BR.
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
- [x] Criar a solution e os cinco projetos vazios, com referências entre eles.
- [x] Protótipo das telas (principal com seleção de empresa, cadastros, emissão CT-e, emissão MDF-e).
  - [x] Principal com seleção de empresa (MDI).
  - [x] Cadastros, fatia 1: Empresa, Participante, Veículo, Proprietário (`Formularios/Cadastros`, lista genérica + diálogo de edição).
  - [x] Cadastros, fatia 2: Certificados, Séries, Motoristas, Apólices.
  - [x] Emissão de CT-e (`Formularios/Cte`: assistente em 5 etapas + consulta).
  - [x] Emissão de MDF-e (`Formularios/Mdfe`: assistente em 5 etapas + consulta).
- [ ] Persistência (SQLite/EF Core, `docs/LEVANTAMENTO.md` §8.2).
  - [x] P1: entidades e regras no Dominio, DbContext, repositório e migração `Inicial` no Dados (`dotnet tool restore` antes de `dotnet ef`).
  - [x] P2a: banco na abertura (`appsettings.json`), municípios do IBGE, empresas e seleção de empresa no banco, dados de demonstração.
  - [x] P2b: participantes, veículos, proprietários, motoristas, apólices e séries no banco (certificados na P3).
  - [x] P2c: assistentes e consultas de CT-e/MDF-e lendo os cadastros do banco (documentos ainda em memória).
  - [x] P3: certificado A1/A3 real (DPAPI na máquina, A3 pelo thumbprint, `IProvedorCertificado`).
  - [ ] P4: CT-e e MDF-e gravados, com o motor DF-e.
    - [x] P4a: CT-e e MDF-e gravados no banco (rascunho e pronto para transmitir), sem XML nem SEFAZ.
    - [x] P4b1: motor DF-e offline do CT-e (classes do XSD, chave, XML, assinatura, validação; "Gerar XML de teste").
    - [x] P4b2: motor DF-e offline do MDF-e (classes do XSD, XML, assinatura, validação; IE/UF do proprietário).
    - [ ] P4c: transmissão em homologação e numeração da série.
      - [x] P4c0: webservices de homologação conferidos nas tabelas oficiais (`docs/LEVANTAMENTO.md` §8.3).
      - [x] P4c1: confiança ICP-Brasil, cliente SOAP com certificado e status do serviço (falta teste real com certificado da empresa).
      - [ ] P4c2: autorização do CT-e em homologação.
      - [ ] P4c3: autorização do MDF-e em homologação.
    - [ ] P4d: eventos (cancelamento, encerramento, inutilização, CC-e).
