using Catalog.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Orders.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Application.Consumer;
public class ProdutoAlteradoConsumer : IConsumer<IProdutoAtualizadoEvent>
{
    private readonly ILogger<ProdutoAlteradoConsumer> _logger;
    private readonly IProdutoCatalogoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ProdutoAlteradoConsumer(ILogger<ProdutoAlteradoConsumer> logger, IProdutoCatalogoRepository repository, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<IProdutoAtualizadoEvent> context)
    {
        var msg = context.Message;

        _logger.LogInformation("Produto atualizado via evento: {Nome}", msg.Id);

        var produto = await _repository.ObterPorIdAsync(msg.Id);
        if (produto is null)
        {
            _logger.LogInformation("Produto não encontrado");
            return;
        }

        produto!.Atualizar(msg.Nome, msg.Categoria, msg.Preco,msg.Disponibilidade);

        await _repository.AtualizarAsync(produto);
        await _unitOfWork.CommitAsync();
    }
}
