using MediatR;
using Orders.Application.DTOs;
using Orders.Domain.Repositories;

namespace Orders.Application.Queries;
public record ObterPedidoPorIdQuery(Guid Id) : IRequest<PedidoOutputDTO?>;

public class ObterPedidoPorIdQueryHandler : IRequestHandler<ObterPedidoPorIdQuery, PedidoOutputDTO?>
{
    private readonly IPedidoRepository _repository;

    public ObterPedidoPorIdQueryHandler(IPedidoRepository repository)
    {
        _repository = repository;
    }

    public async Task<PedidoOutputDTO?> Handle(ObterPedidoPorIdQuery request, CancellationToken cancellationToken)
    {
        var pedido = await _repository.ObterPorIdAsync(request.Id);
        if (pedido == null) 
            return null;

        return new PedidoOutputDTO(
            pedido.Id,
            pedido.ClienteId,
            pedido.DataCriacao,
            pedido.Status.ToString(),
            pedido.CalcularTotal(),
            pedido.Itens.Select(i => new ItemPedidoOutputDTO(
                i.ProdutoId,
                i.NomeProduto,
                i.PrecoUnitario,
                i.Quantidade,
                i.CalcularTotal()
            )).ToList()
        );
    }
}