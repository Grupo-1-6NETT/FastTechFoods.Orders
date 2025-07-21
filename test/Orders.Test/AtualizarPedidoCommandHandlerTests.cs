using Moq;
using Orders.Application.Commands;
using Orders.Application.DTOs;
using Orders.Domain.Entities;
using Orders.Domain.Enums;
using Orders.Domain.Repositories;

namespace Orders.Test;
public class AtualizarPedidoCommandHandlerTests
{
    private readonly Mock<IPedidoRepository> _pedidoRepoMock = new();    
    private readonly Mock<IUnitOfWork> _unitMock = new();
    private readonly Mock<IProdutoCatalogoRepository> _produtoCatalogoRepositoryMock = new();

    private readonly AtualizarPedidoCommandHandler _handler;

    public AtualizarPedidoCommandHandlerTests()
    {
        _handler = new AtualizarPedidoCommandHandler(
            _pedidoRepoMock.Object,
            _unitMock.Object,
            _produtoCatalogoRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Handle_PedidoValido_DeveAtualizarPedido()
    {
        var pedidoId = Guid.NewGuid();
        var clienteId = Guid.NewGuid();
        
        var itens = new List<ItemPedido>
        {
            new(Guid.NewGuid(), "Pizza", 20m, 1)
        };
        var pedido = new Pedido(clienteId, itens);

        var itensDto = new List<ItemPedidoDTO>
        {
            new(Guid.NewGuid(), 3)
        };

        _pedidoRepoMock
            .Setup(x => x.ObterPorIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(pedido);

        _produtoCatalogoRepositoryMock
            .Setup(x => x.ObterPorIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new ProdutoCatalogo(itensDto[0].ProdutoId, "Hamburguer", "Lanche", 14.99m));

        var command = new AtualizarPedidoCommand(clienteId, pedidoId, itensDto);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert        
        Assert.NotNull(result);
        Assert.True(result.Itens.Any());

        _pedidoRepoMock.Verify(r => r.AtualizarAsync(It.IsAny<Pedido>()), Times.Once);
        _unitMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_PedidoInvalido_DeveRetornarFalse()
    {
        var pedidoId = Guid.NewGuid();
        var clienteId = Guid.NewGuid();
        
        var itens = new List<ItemPedido>
        {
            new(Guid.NewGuid(), "Pizza", 20m, 1)
        };
        var pedido = new Pedido(clienteId, itens);

        var itensDto = new List<ItemPedidoDTO>
        {
            new(Guid.NewGuid(), 3)
        };

        _pedidoRepoMock
            .Setup(x => x.ObterPorIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(default(Pedido?));

        var command = new AtualizarPedidoCommand(clienteId, pedidoId, itensDto);

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, default));

        _pedidoRepoMock.Verify(r => r.AtualizarAsync(It.IsAny<Pedido>()), Times.Never);
        _unitMock.Verify(u => u.CommitAsync(), Times.Never);
    }
}