using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orders.Application.Commands;
using Orders.Application.DTOs;
using Orders.Application.Queries;
using Orders.Domain.Enums;

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
    [Authorize(Roles = "cliente")]
    public async Task<IActionResult> CriarPedido([FromBody] CriarPedidoDTO dto)
    {
        try
        {
            var pedido = await _mediator.Send(new CriarPedidoCommand(dto));
            return CreatedAtAction(nameof(GetById), new { id = pedido.Id }, pedido);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "cliente")]
    public async Task<IActionResult> Cancelar(Guid id, [FromQuery] string justificativa)
    {
        if (string.IsNullOrWhiteSpace(justificativa))
            return BadRequest("Justificativa é obrigatória");

        var sucesso = await _mediator.Send(new CancelarPedidoCommand(id, justificativa));
        return sucesso ? Ok("Pedido cancelado") : BadRequest("Não foi possível cancelar o pedido");
    }
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "cliente,gerente,atendente")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var pedido = await _mediator.Send(new ObterPedidoPorIdQuery(id));
        return pedido is not null ? Ok(pedido) : NotFound("Pedido não encontrado");
    }

    [HttpGet("cliente/{clienteId}")]
    [Authorize(Roles = "cliente,gerente,atendente")]
    public async Task<IActionResult> GetByCliente(Guid clienteId)
    {
        var pedidos = await _mediator.Send(new ObterPedidosPorClienteQuery(clienteId));
        return Ok(pedidos);
    }

    [HttpPut("{id}/confirmar")]
    [Authorize(Roles = "atendente")]
    public async Task<IActionResult> Confirmar(Guid id)
    {
        var sucesso = await _mediator.Send(new AtualizarStatusPedidoCommand(id, StatusPedido.Confirmado));
        return sucesso ? Ok("Pedido confirmado") : BadRequest("Erro ao confirmar pedido");
    }

    [HttpPut("{id}/rejeitar")]
    [Authorize(Roles = "atendente")]
    public async Task<IActionResult> Rejeitar(Guid id)
    {
        var sucesso = await _mediator.Send(new AtualizarStatusPedidoCommand(id, StatusPedido.Rejeitado));
        return sucesso ? Ok("Pedido rejeitado") : BadRequest("Erro ao rejeitar pedido");
    }

    [HttpPut("{id}/preparar")]
    [Authorize(Roles = "atendente")]
    public async Task<IActionResult> Preparar(Guid id)
    {
        var sucesso = await _mediator.Send(new AtualizarStatusPedidoCommand(id, StatusPedido.EmPreparacao));
        return sucesso ? Ok("Pedido em preparo") : BadRequest("Erro ao preparar pedido");
    }

    [HttpPut("{id}/finalizar")]
    [Authorize(Roles = "atendente")]
    public async Task<IActionResult> Finalizar(Guid id)
    {
        var sucesso = await _mediator.Send(new AtualizarStatusPedidoCommand(id, StatusPedido.Finalizado));
        return sucesso ? Ok("Pedido finalizado") : BadRequest("Erro ao finalizar pedido");
    }
}
