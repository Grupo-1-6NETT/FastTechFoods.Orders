using MediatR;
using Orders.Application.DTOs;
using Orders.Domain.Entities;
using Orders.Domain.Repositories;

namespace Orders.Application.Commands;

public record AtualizarPedidoCommand(Guid ClientId, Guid PedidoId, List<ItemPedidoDTO> Itens) : IRequest<PedidoOutputDTO>;
public class AtualizarPedidoCommandHandler : IRequestHandler<AtualizarPedidoCommand, PedidoOutputDTO>
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProdutoCatalogoRepository _produtoRepository;


    public AtualizarPedidoCommandHandler(IPedidoRepository pedidoRepository, IUnitOfWork unitOfWork, IProdutoCatalogoRepository produtoRepository)
    {
        _pedidoRepository = pedidoRepository;
        _unitOfWork = unitOfWork;
        _produtoRepository = produtoRepository;
    }

    public async Task<PedidoOutputDTO> Handle(AtualizarPedidoCommand request, CancellationToken cancellationToken)
    {
        var pedido = await _pedidoRepository.ObterPorIdAsync(request.PedidoId);
        if (pedido is null)
            throw new InvalidOperationException($"Pedido não encontrado.");

        if(pedido.ClienteId != request.ClientId)
            throw new InvalidOperationException($"Pedido de outro cliente. Sem autorização para alterar.");


        var itens = new List<ItemPedido>();

        foreach (var itemDto in request.Itens)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(itemDto.ProdutoId);
            if (produto is null)
                throw new InvalidOperationException($"Produto com ID {itemDto.ProdutoId} não encontrado.");

            itens.Add(new ItemPedido(produto.Id, produto.Nome, produto.Preco, itemDto.Quantidade));
        }

        pedido.InserirItens(itens);

        await _pedidoRepository.AtualizarAsync(pedido);
        await _unitOfWork.CommitAsync();        

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
