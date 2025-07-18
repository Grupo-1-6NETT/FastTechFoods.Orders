using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orders.Application.Queries;

namespace Orders.API.Controllers;
[Route("produtos-catalogo")]
[ApiController]
public class ProdutoCatalogoController(IMediator _mediator) : ControllerBase
{
    /// <summary>
    /// Obtem lista de produtos disponiveis
    /// </summary>
    /// <returns>Lista de produtos cadastradoso</returns>
    /// <response code="200">Lista de produtos</response>
    /// <response code="400">Falha no processo</response>
    /// <response code="401">Funcionário não autenticado</response>    
    /// <response code="500">Erro inesperado</response>
    [HttpGet]
    [Authorize(Roles = "cliente,gerente,atendente")]
    public async Task<IActionResult> ObterTodosProdutos()
    {
        var produtos = await _mediator.Send(new ObterProdutosCatalogoQuery());
        return Ok(produtos);
    }

    /// <summary>
    /// Obtem lista de produtos disponiveis
    /// </summary>
    /// <returns>Lista de produtos cadastradoso</returns>
    /// <response code="200">Lista de produtos por categoria</response>
    /// <response code="400">Falha no processo</response>
    /// <response code="401">Funcionário não autenticado</response>    
    /// <response code="500">Erro inesperado</response>
    [HttpGet("{categoria:string}")]
    [Authorize(Roles = "cliente,gerente,atendente")]
    public async Task<IActionResult> ObterProdutosPorCategoria(string categoria)
    {
        var produtos = await _mediator.Send(new ObterProdutosCatalogoQuery());
        var produtosPorcategoria = produtos.Where(p => p.Categoria == categoria);
        return Ok(produtosPorcategoria);
    }
}
