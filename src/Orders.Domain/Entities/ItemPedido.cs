namespace Orders.Domain.Entities;
public class ItemPedido
{
    public Guid ProdutoId { get; private set; }
    public string NomeProduto { get; private set; } = string.Empty;
    public decimal PrecoUnitario { get; private set; }
    public int Quantidade { get; private set; }

    protected ItemPedido() { }

    public ItemPedido(Guid produtoId, string nome, decimal preco, int quantidade)
    {
        ProdutoId = produtoId;
        NomeProduto = nome;
        PrecoUnitario = preco;
        Quantidade = quantidade;
    }

    public decimal CalcularTotal() => PrecoUnitario * Quantidade;
}
