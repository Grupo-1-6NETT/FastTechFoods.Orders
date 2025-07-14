namespace Orders.Application.DTOs;
public record ItemPedidoDTO(Guid ProdutoId, string NomeProduto, decimal PrecoUnitario, int Quantidade);
public record CriarPedidoDTO(Guid ClienteId, List<ItemPedidoDTO> Itens);
