using MediatR;
using Microsoft.AspNetCore.Mvc;
using Orders.Application.Commands;
using Orders.Application.DTOs;
using Orders.Application.Queries;

namespace Orders.API.Controllers;
[Route("[controller]")]
[ApiController]
public class PedidoController : ControllerBase
{
    private readonly IMediator _mediator;

    public PedidoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CriarPedido([FromBody] CriarPedidoDTO dto)
    {
        var id = await _mediator.Send(new CriarPedidoCommand(dto));
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var pedido = await _mediator.Send(new ObterPedidoPorIdQuery(id));
        return pedido is not null ? Ok(pedido) : NotFound();
    }

    [HttpGet("cliente/{clienteId}")]
    public async Task<IActionResult> GetByCliente(Guid clienteId)
    {
        var pedidos = await _mediator.Send(new ObterPedidosPorClienteQuery(clienteId));
        return Ok(pedidos);
    }
}
