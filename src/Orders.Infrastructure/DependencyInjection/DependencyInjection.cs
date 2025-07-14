using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orders.Domain.Repositories;
using Orders.Infrastructure.Data;
using Orders.Infrastructure.Repositories;

namespace Orders.Infrastructure.DependencyInjection;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDI(this IServiceCollection services, IConfiguration config)
    {
        AddDbContext(services, config);
        AddRepositories(services);

        return services;
    }

    private static void AddDbContext(IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<OrderDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("PostgresString")));
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IPedidoRepository, PedidoRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProdutoCatalogoRepository, ProdutoCatalogoRepository>();
    }
}
