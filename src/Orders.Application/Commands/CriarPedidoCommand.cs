using MassTransit;
using MediatR;
using Orders.Application.DTOs;
using Orders.Domain.Entities;
using Orders.Domain.Events;
using Orders.Domain.Repositories;

namespace Orders.Application.Commands;

public record CriarPedidoCommand(CriarPedidoDTO Pedido) : IRequest<PedidoOutputDTO>;
public class CriarPedidoCommandHandler : IRequestHandler<CriarPedidoCommand, PedidoOutputDTO>
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProdutoCatalogoRepository _produtoRepository;
    private readonly IPublishEndpoint _publish;

    public CriarPedidoCommandHandler(IPedidoRepository pedidoRepository, IUnitOfWork unitOfWork, IProdutoCatalogoRepository produtoRepository, IPublishEndpoint publish)
    {
        _pedidoRepository = pedidoRepository;
        _unitOfWork = unitOfWork;
        _produtoRepository = produtoRepository;
        _publish = publish;
    }

    public async Task<PedidoOutputDTO> Handle(CriarPedidoCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Pedido;

        var itens = new List<ItemPedido>();

        foreach (var itemDto in dto.Itens)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(itemDto.ProdutoId);
            if (produto is null)
                throw new InvalidOperationException($"Produto com ID {itemDto.ProdutoId} não encontrado.");

            itens.Add(new ItemPedido(produto.Id, produto.Nome, produto.Preco, itemDto.Quantidade));
        }

        var pedido = new Pedido(dto.ClienteId, itens);

        await _pedidoRepository.AdicionarAsync(pedido);
        await _unitOfWork.CommitAsync();

        await _publish.Publish<IPedidoCriadoEvent>(new
        {
            PedidoId = pedido.Id,
            ClienteId = pedido.ClienteId,
            DataCriacao = pedido.DataCriacao,
            Total = pedido.CalcularTotal(),
            Itens = pedido.Itens.Select(i => new ItemPedidoEventModel(i.ProdutoId, i.NomeProduto, i.Quantidade, i.PrecoUnitario))
        });

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
