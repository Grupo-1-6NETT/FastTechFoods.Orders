namespace Catalog.Domain.Events;
public interface IProdutoAtualizadoEvent
{
    Guid Id { get; }
    string Nome { get; }
    string Categoria { get; }
    decimal Preco { get; }
    bool Disponibilidade { get; }
}
