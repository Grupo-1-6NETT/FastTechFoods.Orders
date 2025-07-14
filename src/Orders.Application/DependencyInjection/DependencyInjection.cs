using Microsoft.Extensions.DependencyInjection;
using Orders.Application.Commands;

namespace Orders.Application.DependencyInjection;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDI(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CriarPedidoCommandHandler).Assembly);
            });

        return services;
    }
}
