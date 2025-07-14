using Microsoft.EntityFrameworkCore;
using Orders.Domain.Entities;

namespace Orders.Infrastructure.Data;
public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions options) : base(options)
    {
    }
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<ProdutoCatalogo> ProdutosCatalogo => Set<ProdutoCatalogo>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pedido>(builder =>
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.DataCriacao).IsRequired();
            builder.Property(p => p.Status).IsRequired();

            builder.OwnsMany(p => p.Itens, itens =>
            {
                itens.WithOwner().HasForeignKey("PedidoId");

                itens.HasKey(i => i.Id); 
                itens.Property(i => i.Id).ValueGeneratedNever();

                itens.Property(i => i.ProdutoId).IsRequired();
                itens.Property(i => i.NomeProduto).IsRequired();
                itens.Property(i => i.PrecoUnitario).HasColumnType("decimal(10,2)");
                itens.Property(i => i.Quantidade).IsRequired();
            });
        });
        modelBuilder.Entity<ProdutoCatalogo>(builder =>
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Nome).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Categoria).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Preco).HasColumnType("decimal(10,2)");
        });
    }

}
