using MediatR;
using Orders.Application.DTOs;
using Orders.Domain.Entities;
using Orders.Domain.Repositories;

namespace Orders.Application.Commands;

public record CriarPedidoCommand(CriarPedidoDTO Pedido) : IRequest<Guid>;
public class CriarPedidoCommandHandler : IRequestHandler<CriarPedidoCommand, Guid>
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CriarPedidoCommandHandler(IPedidoRepository pedidoRepository, IUnitOfWork unitOfWork)
    {
        _pedidoRepository = pedidoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CriarPedidoCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Pedido;

        var itens = dto.Itens.Select(i =>
            new ItemPedido(i.ProdutoId, i.NomeProduto, i.PrecoUnitario, i.Quantidade)).ToList();

        var pedido = new Pedido(dto.ClienteId, itens);

        await _pedidoRepository.AdicionarAsync(pedido);
        await _unitOfWork.CommitAsync();

        return pedido.Id;
    }
}
