using Catalog.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Orders.Application.Consumer;
public class ProdutoCadastradoConsumer : IConsumer<IProdutoCadastradoEvent>
{
    private readonly ILogger<ProdutoCadastradoConsumer> _logger;

    public ProdutoCadastradoConsumer(ILogger<ProdutoCadastradoConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<IProdutoCadastradoEvent> context)
    {
        var produto = context.Message;
        _logger.LogInformation("Produto recebido via RabbitMQ: {Nome}, Categoria: {Categoria}, Preço: {Preco}",
            produto.Nome, produto.Categoria, produto.Preco);
       
        return Task.CompletedTask;
    }
}
