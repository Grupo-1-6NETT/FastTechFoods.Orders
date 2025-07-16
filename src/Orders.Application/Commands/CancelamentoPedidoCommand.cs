using MassTransit;
using MediatR;
using Orders.Domain.Events;
using Orders.Domain.Repositories;

namespace Orders.Application.Commands;
public record CancelarPedidoCommand(Guid PedidoId, string Justificativa) : IRequest<bool>;
public class CancelamentoPedidoCommandHandler : IRequestHandler<CancelarPedidoCommand, bool>
{
    private readonly IPedidoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publish;

    public CancelamentoPedidoCommandHandler(IPedidoRepository repository, IUnitOfWork unitOfWork, IPublishEndpoint publish)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _publish = publish;
    }

    public async Task<bool> Handle(CancelarPedidoCommand request, CancellationToken cancellationToken)
    {
        var pedido = await _repository.ObterPorIdAsync(request.PedidoId);
        if (pedido is null) return false;

        try
        {
            pedido.Cancelar(request.Justificativa);
            await _repository.AtualizarAsync(pedido);
            await _unitOfWork.CommitAsync();

            await _publish.Publish<IPedidoCanceladoEvent>(new
            {
                PedidoId = pedido.Id,
                Justificativa = request.Justificativa,
                DataCancelamento = DateTime.UtcNow
            });

            return true;
        }
        catch
        {
            return false;
        }
    }
}
