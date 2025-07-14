namespace Orders.Application.DTOs;
public record ItemPedidoDTO(Guid ProdutoId, int Quantidade);
public record CriarPedidoDTO(Guid ClienteId, List<ItemPedidoDTO> Itens);
