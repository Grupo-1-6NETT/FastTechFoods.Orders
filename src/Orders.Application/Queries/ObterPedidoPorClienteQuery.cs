using MediatR;
using Orders.Domain.Entities;
using Orders.Domain.Repositories;

namespace Orders.Application.Queries;

public record ObterPedidosPorClienteQuery(Guid ClienteId) : IRequest<IEnumerable<Pedido>>;

public class ObterPedidosPorClienteQueryHandler : IRequestHandler<ObterPedidosPorClienteQuery, IEnumerable<Pedido>>
{
    private readonly IPedidoRepository _repository;

    public ObterPedidosPorClienteQueryHandler(IPedidoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Pedido>> Handle(ObterPedidosPorClienteQuery request, CancellationToken cancellationToken)
    {
        return await _repository.ObterPorClienteAsync(request.ClienteId);
    }
}