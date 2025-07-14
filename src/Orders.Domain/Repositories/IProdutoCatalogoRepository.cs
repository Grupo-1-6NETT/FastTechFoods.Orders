using Orders.Domain.Entities;

namespace Orders.Domain.Repositories;
public interface IProdutoCatalogoRepository
{
    Task<ProdutoCatalogo?> ObterPorIdAsync(Guid id);
    Task AdicionarAsync(ProdutoCatalogo produto);
    Task AtualizarAsync(ProdutoCatalogo produto);
}
