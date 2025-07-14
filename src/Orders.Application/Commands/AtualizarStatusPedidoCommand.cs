using MediatR;
using Orders.Domain.Enums;
using Orders.Domain.Repositories;

public record AtualizarStatusPedidoCommand(Guid PedidoId, StatusPedido NovoStatus) : IRequest<bool>;

public class AtualizarStatusPedidoCommandHandler : IRequestHandler<AtualizarStatusPedidoCommand, bool>
{
    private readonly IPedidoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AtualizarStatusPedidoCommandHandler(IPedidoRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(AtualizarStatusPedidoCommand request, CancellationToken cancellationToken)
    {
        var pedido = await _repository.ObterPorIdAsync(request.PedidoId);
        if (pedido is null) return false;

        try
        {
            pedido.AlterarStatus(request.NovoStatus);
            await _repository.AtualizarAsync(pedido);
            await _unitOfWork.CommitAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}