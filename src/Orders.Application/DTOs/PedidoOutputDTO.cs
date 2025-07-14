namespace Orders.Application.DTOs;
public record ItemPedidoOutputDTO(
    Guid ProdutoId,
    string NomeProduto,
    decimal PrecoUnitario,
    int Quantidade,
    decimal Total
);

public record PedidoOutputDTO(
    Guid Id,
    Guid ClienteId,
    DateTime DataCriacao,
    string Status,
    decimal Total,
    List<ItemPedidoOutputDTO> Itens
);