using Kitchen.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Orders.Domain.Enums;
using Orders.Domain.Repositories;

namespace Orders.Application.Consumer;
public class PedidoFinalizadoConsumer : IConsumer<IPedidoFinalizadoEvent>
{
    private readonly IPedidoRepository _repository;
    private readonly IUnitOfWork _unit;
    private readonly ILogger<PedidoFinalizadoConsumer> _logger;

    public PedidoFinalizadoConsumer(IPedidoRepository repository, IUnitOfWork unit, ILogger<PedidoFinalizadoConsumer> logger)
    {
        _repository = repository;
        _unit = unit;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IPedidoFinalizadoEvent> context)
    {
        var evento = context.Message;

        var pedido = await _repository.ObterPorIdAsync(evento.PedidoId);
        if (pedido is null)
        {
            _logger.LogWarning("Pedido não encontrado: {Id}", evento.PedidoId);
            return;
        }

        pedido.AlterarStatus(StatusPedido.Finalizado);
        await _unit.CommitAsync();

        _logger.LogInformation("Pedido finalizado via evento: {Id}", evento.PedidoId);
    }
}
