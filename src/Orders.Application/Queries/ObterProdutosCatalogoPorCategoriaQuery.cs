using MediatR;
using Orders.Application.DTOs;
using Orders.Domain.Repositories;

namespace Orders.Application.Queries;
public record ObterProdutosCatalogoPorCategoriaQuery(string categoria) : IRequest<IEnumerable<ProdutoCatalogoDTO>>;
public class ObterProdutosCatalogoPorCategoriaQueryHandler : IRequestHandler<ObterProdutosCatalogoPorCategoriaQuery, IEnumerable<ProdutoCatalogoDTO>>
{
    private readonly IProdutoCatalogoRepository _repository;

    public ObterProdutosCatalogoPorCategoriaQueryHandler(IProdutoCatalogoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ProdutoCatalogoDTO>> Handle(ObterProdutosCatalogoPorCategoriaQuery request, CancellationToken cancellationToken)
    {
        var produtos = await _repository.ObterTodosAsync();
        var produtosPorCategoria = produtos.Where(p => p.Categoria == request.categoria).ToList();

        return produtosPorCategoria.Select(p => new ProdutoCatalogoDTO(
            p.Id,
            p.Nome,
            p.Categoria,
            p.Preco
        ));
    }
}
