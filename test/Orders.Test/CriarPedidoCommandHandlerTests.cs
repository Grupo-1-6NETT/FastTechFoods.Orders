using MassTransit;
using Moq;
using Orders.Application.Commands;
using Orders.Application.DTOs;
using Orders.Domain.Entities;
using Orders.Domain.Events;
using Orders.Domain.Repositories;

namespace Orders.Test;
public class CriarPedidoCommandHandlerTests
{
    private readonly Mock<IPedidoRepository> _pedidoRepoMock = new();
    private readonly Mock<IProdutoCatalogoRepository> _produtoRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitMock = new();
    private readonly Mock<IPublishEndpoint> _publishMock = new();

    private readonly CriarPedidoCommandHandler _handler;

    public CriarPedidoCommandHandlerTests()
    {
        _handler = new CriarPedidoCommandHandler(
            _pedidoRepoMock.Object,
            _unitMock.Object,
            _produtoRepoMock.Object
        );
    }

    [Fact]
    public async Task Handle_ComProdutosValidos_DeveCriarPedidoERetornarDTO()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var clienteId = Guid.NewGuid();

        var produto = new ProdutoCatalogo(produtoId, "Coca-Cola", "Bebida", 10m);
        _produtoRepoMock.Setup(r => r.ObterPorIdAsync(produtoId))
                        .ReturnsAsync(produto);

        var dto = new CriarPedidoDTO(clienteId, new List<ItemPedidoDTO>
        {
            new(produtoId, 2)
        });

        var command = new CriarPedidoCommand(clienteId, dto.Itens);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(clienteId, result.ClienteId);
        Assert.Single(result.Itens);
        Assert.Equal(20m, result.Total);

        _pedidoRepoMock.Verify(r => r.AdicionarAsync(It.IsAny<Pedido>()), Times.Once);
        _unitMock.Verify(u => u.CommitAsync(), Times.Once);

    }

    [Fact]
    public async Task Handle_ProdutoNaoEncontrado_DeveLancarExcecao()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var clienteId = Guid.NewGuid();

        _produtoRepoMock.Setup(r => r.ObterPorIdAsync(produtoId))
                        .ReturnsAsync((ProdutoCatalogo?)null);

        var dto = new CriarPedidoDTO(clienteId, new List<ItemPedidoDTO>
        {
            new(produtoId, 1)
        });

        var command = new CriarPedidoCommand(clienteId, dto.Itens);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, default));

        Assert.Contains("Produto com ID", ex.Message);
    }
}