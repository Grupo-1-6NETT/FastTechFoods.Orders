using MassTransit;
using Moq;
using Orders.Application.Commands;
using Orders.Domain.Entities;
using Orders.Domain.Events;
using Orders.Domain.Repositories;

namespace Orders.Test;
public class CancelamentoPedidoCommandHandlerTests
{
    private readonly Mock<IPedidoRepository> _pedidoRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitMock = new();
    private readonly Mock<IPublishEndpoint> _publishMock = new();

    private readonly CancelamentoPedidoCommandHandler _handler;

    public CancelamentoPedidoCommandHandlerTests()
    {
        _handler = new CancelamentoPedidoCommandHandler(
            _pedidoRepoMock.Object,
            _unitMock.Object,
            _publishMock.Object
        );
    }

    [Fact]
    public async Task Handle_PedidoExiste_DeveCancelar()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var justificativa = "Cliente desistiu";

        var pedido = new Pedido(Guid.NewGuid(), new List<ItemPedido>
        {
            new(Guid.NewGuid(), "Pizza", 20m, 1)
        });

        _pedidoRepoMock.Setup(r => r.ObterPorIdAsync(pedidoId))
                       .ReturnsAsync(pedido);

        var command = new CancelarPedidoCommand(pedidoId, justificativa);

        // Act
        var resultado = await _handler.Handle(command, default);

        // Assert
        Assert.True(resultado);
        _pedidoRepoMock.Verify(r => r.AtualizarAsync(pedido), Times.Once);
        _unitMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_PedidoNaoEncontrado_DeveRetornarFalse()
    {
        // Arrange
        var command = new CancelarPedidoCommand(Guid.NewGuid(), "Qualquer motivo");

        _pedidoRepoMock.Setup(r => r.ObterPorIdAsync(command.PedidoId))
                       .ReturnsAsync((Pedido?)null);

        // Act
        var resultado = await _handler.Handle(command, default);

        // Assert
        Assert.False(resultado);
        _pedidoRepoMock.Verify(r => r.AtualizarAsync(It.IsAny<Pedido>()), Times.Never);
        _publishMock.Verify(p => p.Publish<IPedidoCanceladoEvent>(It.IsAny<object>(), default), Times.Never);
    }

    [Fact]
    public async Task Handle_ExcecaoDuranteAtualizacao_DeveRetornarFalse()
    {
        // Arrange
        var pedido = new Pedido(Guid.NewGuid(), new List<ItemPedido>
        {
            new(Guid.NewGuid(), "Suco", 5m, 2)
        });

        var command = new CancelarPedidoCommand(pedido.Id, "Erro forçado");

        _pedidoRepoMock.Setup(r => r.ObterPorIdAsync(command.PedidoId))
                       .ReturnsAsync(pedido);

        _pedidoRepoMock.Setup(r => r.AtualizarAsync(It.IsAny<Pedido>()))
                       .ThrowsAsync(new Exception("Erro ao salvar"));

        // Act
        var resultado = await _handler.Handle(command, default);

        // Assert
        Assert.False(resultado);
        _publishMock.Verify(p => p.Publish<IPedidoCanceladoEvent>(It.IsAny<object>(), default), Times.Never);
    }
}
