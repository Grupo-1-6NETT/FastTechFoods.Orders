using MassTransit;
using MediatR;
using Orders.Domain.Enums;
using Orders.Domain.Events;
using Orders.Domain.Repositories;

public record ConfirmarPedidoCommand(Guid PedidoId, FormaDeEntrega FormaDeEntrega) : IRequest<bool>;

public class ConfirmarPedidoCommandHandler : IRequestHandler<ConfirmarPedidoCommand, bool>
{
    private readonly IPedidoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publish;

    public ConfirmarPedidoCommandHandler(IPedidoRepository repository, IUnitOfWork unitOfWork, IPublishEndpoint publish)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _publish = publish;
    }

    public async Task<bool> Handle(ConfirmarPedidoCommand request, CancellationToken cancellationToken)
    {
        var pedido = await _repository.ObterPorIdAsync(request.PedidoId);
        if (pedido is null) return false;

        try
        {
            pedido.AlterarStatus(StatusPedido.Confirmado);
            pedido.EscolherFormaDeEntrega(request.FormaDeEntrega);
            await _repository.AtualizarAsync(pedido);
            await _unitOfWork.CommitAsync();

            await _publish.Publish<IPedidoCriadoEvent>(new
            {
                PedidoId = pedido.Id,
                ClienteId = pedido.ClienteId,
                DataCriacao = pedido.DataCriacao,
                Total = pedido.CalcularTotal(),
                FormaDeEntrega = pedido.FormaDeEntrega.ToString(),
                Itens = pedido.Itens.Select(i => new ItemPedidoEventModel(i.ProdutoId, i.NomeProduto, i.Quantidade, i.PrecoUnitario))
            });

            return true;
        }
        catch
        {
            return false;
        }
    }
}