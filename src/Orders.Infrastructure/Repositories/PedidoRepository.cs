using Microsoft.EntityFrameworkCore;
using Orders.Domain.Entities;
using Orders.Domain.Repositories;
using Orders.Infrastructure.Data;

namespace Orders.Infrastructure.Repositories;
internal class PedidoRepository : IPedidoRepository
{
    private readonly OrderDbContext _dbContext;

    public PedidoRepository(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AdicionarAsync(Pedido pedido)
    {
        await _dbContext.Pedidos.AddAsync(pedido);
    }

    public async Task AtualizarAsync(Pedido pedido)
    {
        _dbContext.Pedidos.Update(pedido);
    }

    public async Task<Pedido?> ObterPorIdAsync(Guid id)
    {
        return await _dbContext.Pedidos
            .AsNoTracking()
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Pedido>> ObterPorClienteAsync(Guid clienteId)
    {
        return await _dbContext.Pedidos
            .AsNoTracking()
            .Include(p => p.Itens)
            .Where(p => p.ClienteId == clienteId)
            .Take(100)
            .ToListAsync();
    }

    public async Task<IEnumerable<Pedido>> ObterTodosAsync()
    {
        return await _dbContext.Pedidos
            .AsNoTracking()
            .Include(p => p.Itens)
            .Take(100)
            .ToListAsync();
    }
}
