using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Orders.Application.Commands;
using Orders.Application.Consumer;

namespace Orders.Application.DependencyInjection;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDI(this IServiceCollection services)
    {
        AddMediatr(services);
        AddMasstransit(services);

        return services;
    }

    private static void AddMediatr(IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CriarPedidoCommandHandler).Assembly);
        });

    }
    private static void AddMasstransit(IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<ProdutoCadastradoConsumer>();
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint("produto-cadastrado-event", e =>
                {
                    e.ConfigureConsumer<ProdutoCadastradoConsumer>(ctx);
                });
            });
        });
    }
}
