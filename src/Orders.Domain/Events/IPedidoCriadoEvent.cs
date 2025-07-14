namespace Orders.Domain.Events;
public record ItemPedidoEventModel(Guid ProdutoId, string NomeProduto, int Quantidade, decimal PrecoUnitario);
public interface IPedidoCriadoEvent
{
    Guid PedidoId { get; }
    Guid ClienteId { get; }
    DateTime DataCriacao { get; }
    decimal Total { get; }
    IEnumerable<ItemPedidoEventModel> Itens { get; }
}
