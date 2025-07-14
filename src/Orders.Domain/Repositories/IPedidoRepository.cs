using Orders.Domain.Entities;

namespace Orders.Domain.Repositories;
public interface IPedidoRepository
{
    Task<Pedido?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<Pedido>> ObterPorClienteAsync(Guid clienteId);
    Task<IEnumerable<Pedido>> ObterTodosAsync();

    Task AdicionarAsync(Pedido pedido);
    Task AtualizarAsync(Pedido pedido);
}
