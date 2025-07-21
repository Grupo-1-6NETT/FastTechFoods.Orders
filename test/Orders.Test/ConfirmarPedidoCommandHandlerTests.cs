using MassTransit;
using Moq;
using Orders.Domain.Entities;
using Orders.Domain.Enums;
using Orders.Domain.Repositories;

namespace Orders.Test;
public class ConfirmarPedidoCommandHandlerTests
{
    private readonly Mock<IPedidoRepository> _pedidoRepoMock = new();    
    private readonly Mock<IUnitOfWork> _unitMock = new();
    private readonly Mock<IPublishEndpoint> _publishMock = new();

    private readonly ConfirmarPedidoCommandHandler _handler;

    public ConfirmarPedidoCommandHandlerTests()
    {
        _handler = new ConfirmarPedidoCommandHandler(
            _pedidoRepoMock.Object,
            _unitMock.Object,
            _publishMock.Object
        );
    }

    [Fact]
    public async Task Handle_PedidoValido_DeveConfirmarPedido()
    {
        var pedidoId = Guid.NewGuid();
        var formaEntrega = FormaDeEntrega.Delivery;

        var pedido = new Pedido(Guid.NewGuid(), new List<ItemPedido>
        {
            new(Guid.NewGuid(), "Pizza", 20m, 1)
        });

        _pedidoRepoMock
            .Setup(x => x.ObterPorIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(pedido);

        var command = new ConfirmarPedidoCommand(pedidoId, formaEntrega);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert        
        Assert.True(result);
        Assert.Equal(StatusPedido.Confirmado, pedido.Status);
        Assert.Equal(pedido.FormaDeEntrega, formaEntrega);

        _pedidoRepoMock.Verify(r => r.AtualizarAsync(It.IsAny<Pedido>()), Times.Once);
        _unitMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_PedidoInvalido_DeveRetornarFalse()
    {
        var pedidoId = Guid.NewGuid();
        var formaEntrega = FormaDeEntrega.Delivery;

        var pedido = new Pedido(Guid.NewGuid(), new List<ItemPedido>
        {
            new(Guid.NewGuid(), "Pizza", 20m, 1)
        });

        _pedidoRepoMock
            .Setup(x => x.ObterPorIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(default(Pedido?));

        var command = new ConfirmarPedidoCommand(pedidoId, formaEntrega);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert        
        Assert.False(result);
        Assert.NotEqual(StatusPedido.Confirmado, pedido.Status);
        Assert.NotEqual(pedido.FormaDeEntrega, formaEntrega);

        _pedidoRepoMock.Verify(r => r.AtualizarAsync(It.IsAny<Pedido>()), Times.Never);
        _unitMock.Verify(u => u.CommitAsync(), Times.Never);
    }
}