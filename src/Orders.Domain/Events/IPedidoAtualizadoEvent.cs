namespace Orders.Domain.Events;
public interface IPedidoAtualizadoEvent
{
    Guid PedidoId { get; }
    string NovoStatus { get; }
    DateTime DataAtualizacao { get; }
}
