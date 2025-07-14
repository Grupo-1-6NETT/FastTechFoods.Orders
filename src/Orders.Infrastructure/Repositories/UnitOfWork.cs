using Orders.Domain.Repositories;
using Orders.Infrastructure.Data;

namespace Orders.Infrastructure.Repositories;
internal class UnitOfWork : IUnitOfWork
{
    private readonly OrderDbContext _dbContext;

    public UnitOfWork(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CommitAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
