using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orders.Application.Commands;
using Orders.Application.DTOs;
using Orders.Application.Queries;
using Orders.Domain.Enums;
using System.Security.Claims;

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

    /// <summary>
    /// Cria um pedido novo
    /// </summary>
    /// <returns>Pedido criado</returns>
    /// <response code="200">Pedido criado com sucesso</response>
    /// <response code="401">Cliente não autenticado</response>    
    /// <response code="500">Erro inesperado</response>
    [HttpPost]
    [Authorize(Roles = "cliente")]
    public async Task<IActionResult> CriarPedido([FromBody] CriarPedidoDTO dto)
    {
        try
        {
            var clienteId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(clienteId) || !Guid.TryParse(clienteId, out var clienteGuid))
                return Unauthorized(new { erro = "Identificação do cliente inválida" });

            var pedido = await _mediator.Send(new CriarPedidoCommand(clienteGuid, dto.Itens));
            return CreatedAtAction(nameof(GetById), new { id = pedido.Id }, pedido);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    /// <summary>
    /// Consulta pedidos de um cliente
    /// </summary>
    /// <returns>Lista de pedidos do cliente</returns>
    /// <response code="200">Lista de pedidos</response>
    /// <response code="401">Usuário não autenticado</response>    
    /// <response code="500">Erro inesperado</response>
    [HttpGet()]
    [Authorize(Roles = "cliente")]
    public async Task<IActionResult> GetByCliente()
    {
        try
        {
            var clienteId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(clienteId) || !Guid.TryParse(clienteId, out var clienteGuid))
                return Unauthorized(new { erro = "Identificação do cliente inválida" });

            var pedidos = await _mediator.Send(new ObterPedidosPorClienteQuery(clienteGuid));
            return Ok(pedidos);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }

    }

    /// <summary>
    /// Altera itens um pedido novo
    /// </summary>
    /// <returns>Pedido alterado</returns>
    /// <response code="200">Pedido alterado com sucesso</response>
    /// <response code="400">Falha no processo</response>
    /// <response code="401">Cliente não autenticado</response>    
    /// <response code="500">Erro inesperado</response>
    [HttpPost("{PedidoId:guid}")]
    [Authorize(Roles = "cliente")]
    public async Task<IActionResult> AlterarPedido(Guid PedidoId, [FromBody] CriarPedidoDTO dto)
    {
        try
        {
            var clienteId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(clienteId) || !Guid.TryParse(clienteId, out var clienteGuid))
                return Unauthorized(new { erro = "Identificação do cliente inválida" });

            var pedido = await _mediator.Send(new AtualizarPedidoCommand(clienteGuid, PedidoId, dto.Itens));
            return Ok(pedido);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }


    /// <summary>
    /// Confirma o pedido e envia para cozinha
    /// </summary>
    /// <remarks>   
    ///         
    /// Forma de entrega: Balcão = 0, Drive-Thru = 1, Delivery = 2  
    ///         
    /// </remarks>      
    /// <param name="id">ID do pedido</param>
    /// <param name="formaDeEntrega">Forma de entrega</param>
    /// <returns>Status do processo</returns>
    /// <response code="200">Pedido confirmado</response>
    /// <response code="400">Erro ao confirmar pedido</response>
    /// <response code="401">Funcionário não autenticado</response>    
    /// <response code="500">Erro inesperado</response>
    [HttpPut("{id}/confirmar")]
    [Authorize(Roles = "cliente")]
    public async Task<IActionResult> Confirmar(Guid id, [FromQuery]FormaDeEntrega formaDeEntrega)
    {
        var sucesso = await _mediator.Send(new ConfirmarPedidoCommand(id, formaDeEntrega));
        return sucesso ? Ok("Pedido confirmado") : BadRequest("Erro ao confirmar pedido");
    }

    /// <summary>
    /// Cancela um pedido não iniciado preparo
    /// </summary>
    /// <param name="idPedido">ID pedido</param>
    /// <param name="justificativa">Justificativa do cancelamento</param>
    /// <returns>O token de autenticação da API</returns>
    /// <response code="200">Token gerado com sucesso</response>
    /// <response code="401">Funcionário não autenticado</response>    
    /// <response code="500">Erro inesperado</response>
    [HttpDelete()]
    [Authorize(Roles = "cliente")]
    public async Task<IActionResult> Cancelar([FromQuery] Guid idPedido, [FromBody]string justificativa)
    {
        if (string.IsNullOrWhiteSpace(justificativa))
            return BadRequest("Justificativa é obrigatória");

        var sucesso = await _mediator.Send(new CancelarPedidoCommand(idPedido, justificativa));
        return sucesso ? Ok("Pedido cancelado") : BadRequest("Não foi possível cancelar o pedido");
    }

    /// <summary>
    /// Localiza pedido informando ID
    /// </summary>
    /// <param name="id">ID pedido</param>
    /// <returns>Informação do pedido</returns>
    /// <response code="200">Pedido</response>
    /// <response code="404">Pedido não encontrado</response>
    /// <response code="401">Usuário não autenticado</response>    
    /// <response code="500">Erro inesperado</response>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "cliente,gerente,atendente")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var pedido = await _mediator.Send(new ObterPedidoPorIdQuery(id));
        return pedido is not null ? Ok(pedido) : NotFound("Pedido não encontrado");
    }


}
