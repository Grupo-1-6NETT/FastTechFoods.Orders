namespace Orders.Domain.Events;
public interface IPedidoCanceladoEvent
{
    Guid PedidoId { get; }
    string Justificativa { get; }
    DateTime DataCancelamento { get; }
}
