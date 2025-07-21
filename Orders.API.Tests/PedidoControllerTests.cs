using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Orders.API.Controllers;
using Orders.Application.Commands;
using Orders.Application.DTOs;
using Orders.Application.Queries;
using Orders.Domain.Enums;
using System.Security.Claims;
using System.Security.Principal;

namespace Orders.API.Tests;

public class PedidoControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly PedidoController _sut;

    private Guid _clienteId;

    private CriarPedidoDTO PedidoDto = new(new List<ItemPedidoDTO>
            {
                new ItemPedidoDTO(Guid.NewGuid(), 1)
            }
        );

    private PedidoOutputDTO _pedidoOutputDto;

    public PedidoControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _sut = new(_mediatorMock.Object);

        _clienteId = Guid.NewGuid();

        var claims = new List<Claim>()
        {
            new(ClaimTypes.Role, "cliente"),
            new(ClaimTypes.Name, "username"),
            new(ClaimTypes.NameIdentifier, _clienteId.ToString()),
            new("name", "John Doe"),
        };

        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var userMock = new Mock<IPrincipal>();
        userMock
            .Setup(x => x.IsInRole("cliente"))
            .Returns(true);

        var httpContextMock = new Mock<HttpContext>();
        httpContextMock
            .Setup(x => x.User)
            .Returns(claimsPrincipal);

        _sut.ControllerContext.HttpContext = httpContextMock.Object;

        _pedidoOutputDto = new(Guid.NewGuid(), _clienteId, DateTime.Now, "Criado", 10.0m, new() { });
    }

    #region CriarPedido
    [Fact]
    public async Task CriarPedido_InformadosDadosValidos_DeveRetornarCreatedAtActionResult()
    {
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CriarPedidoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_pedidoOutputDto);

        var result = await _sut.CriarPedido(PedidoDto);
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task CriarPedido_DadosInvalidos_DeveRetornarBadRequest()
    {
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CriarPedidoCommand>(), It.IsAny<CancellationToken>()))
            .Throws(new InvalidOperationException());

        var result = await _sut.CriarPedido(PedidoDto);

        Assert.IsType<BadRequestObjectResult>(result);
    }
    #endregion

    #region GetByCliente
    [Fact]
    public async Task GetByCliente_DeveRetornarPedido()
    {
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ObterPedidosPorClienteQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PedidoOutputDTO> { _pedidoOutputDto });

        var result = await _sut.GetByCliente();
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetByCliente_Erro_DeveRetornarBadRequest()
    {
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ObterPedidosPorClienteQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException());

        var result = await _sut.GetByCliente();
        Assert.IsType<BadRequestObjectResult>(result);
    }
    #endregion 

    #region AlterarPedido
    [Fact]
    public async Task AlterarPedido_DadosValidos_DeveRetornarPedidoAlterado()
    {
        var pedidoId = Guid.NewGuid();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<AtualizarPedidoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_pedidoOutputDto);

        var result = await _sut.AlterarPedido(pedidoId, PedidoDto);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task AlterarPedido_Erro_DeveRetornarBadRequest()
    {
        var pedidoId = Guid.NewGuid();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<AtualizarPedidoCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException());

        var result = await _sut.AlterarPedido(pedidoId, PedidoDto);
        Assert.IsType<BadRequestObjectResult>(result);
    }
    #endregion

    #region Confirmar
    [Fact]
    public async Task Confirmar_DadosValidos_DeveConfirmarPedido()
    {
        var pedidoId = Guid.NewGuid();
        var entrega = FormaDeEntrega.Delivery;

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ConfirmarPedidoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Confirmar(pedidoId, entrega);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Confirmar_Erro_DeveRetornarBadRequest()
    {
        var pedidoId = Guid.NewGuid();
        var entrega = FormaDeEntrega.Delivery;

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ConfirmarPedidoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Confirmar(pedidoId, entrega);
        Assert.IsType<BadRequestObjectResult>(result);
    }
    #endregion

    #region Cancelar
    [Fact]
    public async Task Cancelar_DadosValidos_DeveCancelarPedido()
    {
        var pedidoId = Guid.NewGuid();
        var justificativa = "justificativa";

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CancelarPedidoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Cancelar(pedidoId, justificativa);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Cancelar_Erro_DeveRetornarBadRequest()
    {
        var pedidoId = Guid.NewGuid();
        var justificativa = "justificativa";

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CancelarPedidoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Cancelar(pedidoId, justificativa);
        Assert.IsType<BadRequestObjectResult>(result);
    }
    #endregion


    #region GetById
    [Fact]
    public async Task GetById_DadosValidos_DeveRetornarPedido()
    {
        var pedidoId = Guid.NewGuid();        

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ObterPedidoPorIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_pedidoOutputDto);

        var result = await _sut.GetById(pedidoId);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetById_Erro_DeveRetornarBadRequest()
    {
        var pedidoId = Guid.NewGuid();        

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ObterPedidoPorIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(default(PedidoOutputDTO?));

        var result = await _sut.GetById(pedidoId);
        Assert.IsType<NotFoundObjectResult>(result);
    }
    #endregion
}