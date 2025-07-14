using Kitchen.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Orders.Domain.Enums;
using Orders.Domain.Repositories;

namespace Orders.Application.Consumer;
public class PedidoRejeitadoConsumer : IConsumer<IPedidoRejeitadoEvent>
{
    private readonly IPedidoRepository _repository;
    private readonly IUnitOfWork _unit;
    private readonly ILogger<PedidoFinalizadoConsumer> _logger;

    public PedidoRejeitadoConsumer(IPedidoRepository repository, IUnitOfWork unit, ILogger<PedidoFinalizadoConsumer> logger)
    {
        _repository = repository;
        _unit = unit;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IPedidoRejeitadoEvent> context)
    {
        var evento = context.Message;

        var pedido = await _repository.ObterPorIdAsync(evento.PedidoId);
        if (pedido is null)
        {
            _logger.LogWarning("Pedido não encontrado: {Id}", evento.PedidoId);
            return;
        }

        pedido.AlterarStatus(StatusPedido.Cancelado);
        await _unit.CommitAsync();

        _logger.LogInformation("Pedido cancelado via evento: {Id}| Motivo: {Motivo}", evento.PedidoId, evento.Motivo);
    }
}
