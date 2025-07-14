using MediatR;
using Orders.Domain.Entities;
using Orders.Domain.Repositories;

namespace Orders.Application.Queries;
public record ObterPedidoPorIdQuery(Guid Id) : IRequest<Pedido?>;

public class ObterPedidoPorIdQueryHandler : IRequestHandler<ObterPedidoPorIdQuery, Pedido?>
{
    private readonly IPedidoRepository _repository;

    public ObterPedidoPorIdQueryHandler(IPedidoRepository repository)
    {
        _repository = repository;
    }

    public async Task<Pedido?> Handle(ObterPedidoPorIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.ObterPorIdAsync(request.Id);
    }
}