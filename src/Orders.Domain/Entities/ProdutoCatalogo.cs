namespace Orders.Domain.Entities;
public class ProdutoCatalogo
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Categoria { get; private set; } = string.Empty;
    public decimal Preco { get; private set; }

    protected ProdutoCatalogo() { }

    public ProdutoCatalogo(Guid id, string nome, string categoria, decimal preco)
    {
        Id = id;
        Nome = nome;
        Categoria = categoria;
        Preco = preco;
    }

    public void Atualizar(string nome, string categoria, decimal preco)
    {
        Nome = nome;
        Categoria = categoria;
        Preco = preco;
    }
}
