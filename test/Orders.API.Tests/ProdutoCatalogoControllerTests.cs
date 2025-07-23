using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Orders.API.Controllers;
using Orders.Application.DTOs;
using Orders.Application.Queries;

namespace Orders.API.Tests;

public class ProdutoCatalogoControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ProdutoCatalogoController _sut;

    public ProdutoCatalogoControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _sut = new(_mediatorMock.Object);
    }

    #region ObterTodosProdutos
    [Fact]
    public async Task ObterTodosProdutos_DeveRetornarListaProdutos()
    {
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ObterProdutosCatalogoQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _sut.ObterTodosProdutos();
        Assert.IsType<OkObjectResult>(result);
    }
    #endregion

    #region ObterProdutosPorCategoria
    [Fact]
    public async Task ObterProdutosPorCategoria_DeveRetornarListaProdutos()
    {
        var categoria = "categoria";

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ObterProdutosCatalogoQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new ProdutoCatalogoDTO(Guid.NewGuid(), "Produto1", "categoria", 10.0m),
                new ProdutoCatalogoDTO(Guid.NewGuid(), "Produto2", "categoria2", 9.0m),
                ]);

        var result = await _sut.ObterProdutosPorCategoria(categoria);

        var resultObject = Assert.IsType<OkObjectResult>(result);
    }
    #endregion
}