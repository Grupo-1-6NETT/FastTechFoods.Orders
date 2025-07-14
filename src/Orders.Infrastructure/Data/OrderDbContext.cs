using Microsoft.EntityFrameworkCore;
using Orders.Domain.Entities;

namespace Orders.Infrastructure.Data;
public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions options) : base(options)
    {
    }
    public DbSet<Pedido> Pedidos { get; set; }


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
                itens.Property<Guid>("Id");
                itens.HasKey("Id");
                itens.Property(i => i.NomeProduto).IsRequired();
                itens.Property(i => i.PrecoUnitario).HasColumnType("decimal(10,2)");
                itens.Property(i => i.Quantidade).IsRequired();
            });
        });
    }

}
