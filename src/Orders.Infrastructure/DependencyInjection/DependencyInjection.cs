using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

    public static IHost ApplyMigrations(this IHost app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var servicesProvider = scope.ServiceProvider;
            var dbContext = servicesProvider.GetRequiredService<OrderDbContext>();
            try
            {
                if (dbContext.Database.GetPendingMigrations().Any())
                    dbContext.Database.Migrate();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Falha ao conectar com banco de dados | {ex.Message}");
            }
        }
        return app;
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
