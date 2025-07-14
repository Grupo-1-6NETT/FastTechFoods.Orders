using MediatR;
using Orders.Application.DTOs;
using Orders.Domain.Repositories;

namespace Orders.Application.Queries;
public record ObterProdutosCatalogoQuery() : IRequest<IEnumerable<ProdutoCatalogoDTO>>;
public class ObterProdutosCatalogoQueryHandler : IRequestHandler<ObterProdutosCatalogoQuery, IEnumerable<ProdutoCatalogoDTO>>
{
    private readonly IProdutoCatalogoRepository _repository;

    public ObterProdutosCatalogoQueryHandler(IProdutoCatalogoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ProdutoCatalogoDTO>> Handle(ObterProdutosCatalogoQuery request, CancellationToken cancellationToken)
    {
        var produtos = await _repository.ObterTodosAsync();

        return produtos.Select(p => new ProdutoCatalogoDTO(
            p.Id,
            p.Nome,
            p.Categoria,
            p.Preco
        ));
    }
}
