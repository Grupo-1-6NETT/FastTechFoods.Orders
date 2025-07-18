namespace Kitchen.Domain.Events;
public interface IPedidoEmPreparoEvent
{
    Guid PedidoId { get; }
    DateTime DataInicioPreparo { get; }
}
