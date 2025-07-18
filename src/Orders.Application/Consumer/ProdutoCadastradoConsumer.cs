using Catalog.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Orders.Domain.Entities;
using Orders.Domain.Repositories;

namespace Orders.Application.Consumer;
public class ProdutoCadastradoConsumer : IConsumer<IProdutoCadastradoEvent>
{
    private readonly ILogger<ProdutoCadastradoConsumer> _logger;
    private readonly IProdutoCatalogoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ProdutoCadastradoConsumer(ILogger<ProdutoCadastradoConsumer> logger, IProdutoCatalogoRepository repository, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<IProdutoCadastradoEvent> context)
    {
        var msg = context.Message;

        _logger.LogInformation("Produto recebido via evento: {Nome} ({Categoria})", msg.Nome, msg.Categoria);

        var existente = await _repository.ObterPorIdAsync(msg.Id);
        if (existente is null)
        {
            var novo = new ProdutoCatalogo(msg.Id, msg.Nome, msg.Categoria, msg.Preco);
            await _repository.AdicionarAsync(novo);
        }
        else
        {
            existente.Atualizar(msg.Nome, msg.Categoria, msg.Preco, msg.Disponibilidade);
            await _repository.AtualizarAsync(existente);
        }

        await _unitOfWork.CommitAsync();
    }
}
