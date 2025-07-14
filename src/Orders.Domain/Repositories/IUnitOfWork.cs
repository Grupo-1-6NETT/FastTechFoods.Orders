namespace Orders.Domain.Repositories;
public interface IUnitOfWork
{
    Task CommitAsync();
}
