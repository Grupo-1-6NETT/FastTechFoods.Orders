using MediatR;
using Orders.Application.DTOs;
using Orders.Domain.Repositories;

namespace Orders.Application.Queries;

public record ObterPedidosPorClienteQuery(Guid ClienteId) : IRequest<IEnumerable<PedidoOutputDTO>>;

public class ObterPedidosPorClienteQueryHandler : IRequestHandler<ObterPedidosPorClienteQuery, IEnumerable<PedidoOutputDTO>>
{
    private readonly IPedidoRepository _repository;

    public ObterPedidosPorClienteQueryHandler(IPedidoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PedidoOutputDTO>> Handle(ObterPedidosPorClienteQuery request, CancellationToken cancellationToken)
    {
        var pedidos =  await _repository.ObterPorClienteAsync(request.ClienteId);
        return pedidos.Select(p => new PedidoOutputDTO(
             p.Id,
            p.ClienteId,
            p.DataCriacao,
            p.Status.ToString(),
            p.CalcularTotal(),
            p.Itens.Select(i => new ItemPedidoOutputDTO(
                i.ProdutoId,
                i.NomeProduto,
                i.PrecoUnitario,
                i.Quantidade,
                i.CalcularTotal()
            )).ToList())
        );
    }
}