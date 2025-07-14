using MediatR;
using Microsoft.AspNetCore.Mvc;
using Orders.Application.Queries;

namespace Orders.API.Controllers;
[Route("produtos-catalogo")]
[ApiController]
public class ProdutoCatalogoController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ObterTodosProdutos()
    {
        var produtos = await _mediator.Send(new ObterProdutosCatalogoQuery());
        return Ok(produtos);
    }
}
