namespace Emissor.Dominio.Enums;

/// <summary>
/// Papel do veículo no MDF-e: define se vai no grupo <c>veicTracao</c> ou <c>veicReboque</c>.
/// Não é um código do leiaute; os valores são internos do sistema.
/// </summary>
public enum TipoVeiculo
{
    /// <summary>Veículo de tração (grupo <c>veicTracao</c>).</summary>
    Tracao = 1,

    /// <summary>Reboque (grupo <c>veicReboque</c>; até 3 por MDF-e).</summary>
    Reboque = 2,
}
