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
        Status = StatusPedido.Cancelado;
        JustificativaCancelamento = justificativa;
    }

    public void Confirmar() => Status = StatusPedido.Confirmado;
    public void Rejeitar() => Status = StatusPedido.Rejeitado;
    public void Preparar() => Status = StatusPedido.EmPreparacao;
    public void Finalizar() => Status = StatusPedido.Finalizado;
}
