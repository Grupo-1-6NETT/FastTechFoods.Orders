using Orders.Domain.Enums;

namespace Orders.Domain.Entities;
public class Pedido
{
    public Guid Id { get; private set; }
    public Guid ClienteId { get; private set; }
    public DateTime DataCriacao { get; init; } = DateTime.UtcNow;
    public StatusPedido Status { get; private set; } = StatusPedido.Criado;
    public string? JustificativaCancelamento { get; private set; }

    private readonly List<ItemPedido> _itens = new();
    public IReadOnlyCollection<ItemPedido> Itens => _itens;

    protected Pedido() { }

    public Pedido(Guid clienteId, IEnumerable<ItemPedido> itens)
    {
        Id = Guid.NewGuid();
        ClienteId = clienteId;
        _itens.AddRange(itens);
    }

    public decimal CalcularTotal() => _itens.Sum(i => i.CalcularTotal());

    public void Cancelar(string justificativa)
    {
        if (Status != StatusPedido.Criado && Status != StatusPedido.AguardandoConfirmacao)
            throw new InvalidOperationException("Pedido não pode ser cancelado nesse estado.");

        Status = StatusPedido.Cancelado;
        JustificativaCancelamento = justificativa;
    }

    public void AlterarStatus(StatusPedido novoStatus)
    {
        switch (novoStatus)
        {
            case StatusPedido.Confirmado:
                if (Status != StatusPedido.Criado && Status != StatusPedido.AguardandoConfirmacao)
                    throw new InvalidOperationException("Pedido não pode ser confirmado nesse estado.");
                Status = StatusPedido.Confirmado;
                break;

            case StatusPedido.Rejeitado:
                if (Status != StatusPedido.Criado && Status != StatusPedido.AguardandoConfirmacao)
                    throw new InvalidOperationException("Pedido não pode ser rejeitado nesse estado.");
                Status = StatusPedido.Rejeitado;
                break;

            case StatusPedido.EmPreparacao:
                if (Status != StatusPedido.Confirmado)
                    throw new InvalidOperationException("Pedido só pode ir para preparo se estiver confirmado.");
                Status = StatusPedido.EmPreparacao;
                break;

            case StatusPedido.Finalizado:
                if (Status != StatusPedido.EmPreparacao)
                    throw new InvalidOperationException("Pedido só pode ser finalizado após o preparo.");
                Status = StatusPedido.Finalizado;
                break;

            default:
                throw new InvalidOperationException("Status inválido para transição.");
        }
    }
}
