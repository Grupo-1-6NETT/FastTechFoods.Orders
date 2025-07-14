namespace Catalog.Domain.Events;
public interface IProdutoCadastradoEvent
{
    Guid Id { get; }
    string Nome { get; }
    string Categoria { get; }
    decimal Preco { get; }
}
