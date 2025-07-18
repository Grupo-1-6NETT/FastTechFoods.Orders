using Kitchen.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Orders.Domain.Enums;
using Orders.Domain.Repositories;

namespace Orders.Application.Consumer;
public class PedidoEmPreparoConsumer : IConsumer<IPedidoEmPreparoEvent>
{
    private readonly IPedidoRepository _repository;
    private readonly IUnitOfWork _unit;
    private readonly ILogger<PedidoEmPreparoConsumer> _logger;

    public PedidoEmPreparoConsumer(IPedidoRepository repository, IUnitOfWork unit, ILogger<PedidoEmPreparoConsumer> logger)
    {
        _repository = repository;
        _unit = unit;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IPedidoEmPreparoEvent> context)
    {
        var evento = context.Message;

        var pedido = await _repository.ObterPorIdAsync(evento.PedidoId);
        if (pedido is null)
        {
            _logger.LogWarning("Pedido não encontrado: {Id}", evento.PedidoId);
            return;
        }

        pedido.AlterarStatus(StatusPedido.EmPreparacao);
        await _unit.CommitAsync();

        _logger.LogInformation("Iniciado preparo do pedido via evento: {Id}", evento.PedidoId);
    }
}
