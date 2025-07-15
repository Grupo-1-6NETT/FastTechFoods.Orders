using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orders.Application.Commands;
using Orders.Application.Consumer;

namespace Orders.Application.DependencyInjection;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDI(this IServiceCollection services, IConfiguration config)
    {
        AddMediatr(services);
        AddMasstransit(services, config);

        return services;
    }

    private static void AddMediatr(IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CriarPedidoCommandHandler).Assembly);
        });

    }
    private static void AddMasstransit(IServiceCollection services, IConfiguration config)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<ProdutoCadastradoConsumer>();
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username(config["RabbitMQSettings:Username"]!);
                    h.Password(config["RabbitMQSettings:Password"]!);
                });

                cfg.ReceiveEndpoint("produto-cadastrado-event", e =>
                {
                    e.ConfigureConsumer<ProdutoCadastradoConsumer>(ctx);
                });
            });
        });
    }
}
