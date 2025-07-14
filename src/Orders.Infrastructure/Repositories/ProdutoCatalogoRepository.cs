using Microsoft.EntityFrameworkCore;
using Orders.Domain.Entities;
using Orders.Domain.Repositories;
using Orders.Infrastructure.Data;

namespace Orders.Infrastructure.Repositories;
internal class ProdutoCatalogoRepository : IProdutoCatalogoRepository
{
    private readonly OrderDbContext _dbContext;

    public ProdutoCatalogoRepository(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IEnumerable<ProdutoCatalogo>> ObterTodosAsync()
    {
        return await _dbContext.ProdutosCatalogo
            .AsNoTracking()
            .Take(100)
            .ToListAsync();
    }
    public async Task AdicionarAsync(ProdutoCatalogo produto)
    {
        await _dbContext.ProdutosCatalogo.AddAsync(produto);
    }

    public Task AtualizarAsync(ProdutoCatalogo produto)
    {
        _dbContext.ProdutosCatalogo.Update(produto);
        return Task.CompletedTask;
    }

    public async Task<ProdutoCatalogo?> ObterPorIdAsync(Guid id)
    {
        return await _dbContext.ProdutosCatalogo
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

}
