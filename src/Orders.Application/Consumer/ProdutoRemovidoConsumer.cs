using Catalog.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Orders.Domain.Repositories;

namespace Orders.Application.Consumer;
public class ProdutoRemovidoConsumer : IConsumer<IProdutoRemovidoEvent>
{
    private readonly ILogger<ProdutoRemovidoConsumer> _logger;
    private readonly IProdutoCatalogoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ProdutoRemovidoConsumer(ILogger<ProdutoRemovidoConsumer> logger, IProdutoCatalogoRepository repository, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<IProdutoRemovidoEvent> context)
    {
        var msg = context.Message;

        _logger.LogInformation("Produto removido via evento: {Nome}", msg.Id);

        var produto = await _repository.ObterPorIdAsync(msg.Id);
        if (produto is null)
        {
            _logger.LogInformation("Produto não encontrado");
            return;
        }

        await _repository.RemoverAsync(produto!);
        await _unitOfWork.CommitAsync();
    }
}
