using MediatR;
using NerdStore.Core.DomainObjects;
using NerdStore.Vendas.Application.Events;
using NerdStore.Vendas.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NerdStore.Vendas.Application.Commands
{
    public class PedidoCommandHandler : IRequestHandler<AdicionarItemPedidoCommand, bool>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IMediator _mediator;

        public PedidoCommandHandler(IPedidoRepository pedidoRepository, IMediator mediator)
        {
            _pedidoRepository = pedidoRepository;
            _mediator = mediator;
        }

        public async Task<bool> Handle(AdicionarItemPedidoCommand request, CancellationToken cancellationToken)
        {

            if (!request.EhValido())
            {
                foreach (var error in request.ValidationResult.Errors)
                {
                    await _mediator.Publish(new DomainNotification(request.MessageType, error.ErrorMessage), cancellationToken);
                }
                return false;
            }

            var pedido = await _pedidoRepository.ObterPedidoRascunhoPorClienteId(request.ClienteId);
            var pedidoItem = new PedidoItem(produtoId: request.ProdutoId, quantidade: request.Quantidade,
                    valorUnitario: request.ValorUnitario,
                    produtoNome: request.Nome);

            if (pedido == null)
            {
                pedido = Pedido.PedidoFactory.NovoPedidoRascunho(request.ClienteId);
                pedido.AdicionarItem(pedidoItem);

                _pedidoRepository.Adicionar(pedido);
            }
            else
            {
                var pedidoItemExistente = pedido.PedidoItemExistente(pedidoItem);
                pedido.AdicionarItem(pedidoItem);

                if (pedidoItemExistente)
                {
                    _pedidoRepository.AtualizarItem(pedido.PedidoItems.FirstOrDefault(p => p.ProdutoId == pedidoItem.ProdutoId));
                }
                else
                {
                    _pedidoRepository.AdicionarItem(pedidoItem);
                }

                _pedidoRepository.Atualizar(pedido);
            }

            pedido.AdicionarEvento(new PedidoItemAdicionadoEvent(clienteId: request.ClienteId, produtoId: request.ProdutoId,
                 quantidade: request.Quantidade,
                 valorUnitario: request.ValorUnitario,
                 produtoNome: request.Nome, pedidoId: pedido.Id));

            return await _pedidoRepository.UnitOfWork.Commit();
        }
    }
}
