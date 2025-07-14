namespace Kitchen.Domain.Events;
public interface IPedidoRejeitadoEvent
{
    Guid PedidoId { get; }
    string Motivo { get; }
    DateTime DataCancelamento { get; }
}
