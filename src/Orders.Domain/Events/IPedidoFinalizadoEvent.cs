namespace Kitchen.Domain.Events;
public interface IPedidoFinalizadoEvent
{
    Guid PedidoId { get; }
    DateTime DataFinalizacao { get; }
}
