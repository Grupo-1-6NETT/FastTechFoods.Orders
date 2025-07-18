namespace Orders.Application.DTOs;
public record ItemPedidoDTO(Guid ProdutoId, int Quantidade);
public record CriarPedidoDTO(List<ItemPedidoDTO> Itens);
