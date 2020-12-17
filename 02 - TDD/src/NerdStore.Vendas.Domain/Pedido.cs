using NerdStore.Core.DomainObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NerdStore.Vendas.Domain
{
    public class Pedido
    {
        public static int MAX_UNIDADES_ITEM => 15;
        public static int MIN_UNIDADES_ITEM => 1;

        protected Pedido() { }

        public Guid ClienteId { get; private set; }
        public PedidoStatus PedidoStatus { get; private set; }
        public decimal ValorTotal { get; private set; }
        public IList<PedidoItem> PedidoItems { get; set; } = new List<PedidoItem>();

        public void AdicionarItem(PedidoItem pedidoItem)
        {
            ValidarQuantidadeItemPermitida(pedidoItem);

            if (PedidoItemExistente(pedidoItem))
            {                
                var itemExistente = PedidoItems.FirstOrDefault(p => p.ProdutoId == pedidoItem.ProdutoId);
      
                itemExistente.AdicionarUnidades(pedidoItem.Quantidade);
                pedidoItem = itemExistente;

                PedidoItems.Remove(itemExistente);
            }

            PedidoItems.Add(pedidoItem);
            CalcularValorPedido();
        }

        private void ValidarQuantidadeItemPermitida(PedidoItem item)
        {
            var quantidadeItens = item.Quantidade;

            if (PedidoItemExistente(item))
            {
                var itemExistente = PedidoItems.FirstOrDefault(p => p.ProdutoId == item.ProdutoId);
                quantidadeItens += itemExistente.Quantidade;
            }

            if (quantidadeItens > MAX_UNIDADES_ITEM)
            {
                throw new DomainExeciption($"Máximo de {MAX_UNIDADES_ITEM} unidades por produto");
            }
        }

        private bool PedidoItemExistente(PedidoItem item)
        {
            return PedidoItems.Any(p => p.ProdutoId == item.ProdutoId);
        }

        private void CalcularValorPedido()
        {
            ValorTotal = PedidoItems.Sum(i => i.CalcularValor());
        }

        public void TornarRascunho()
        {
            PedidoStatus = PedidoStatus.Rascunho;
        }

        public static class PedidoFactory
        {
            public static Pedido NovoPedidoRascunho(Guid clienteId)
            {
                var pedido = new Pedido
                {
                    ClienteId = clienteId
                };

                pedido.TornarRascunho();
                return pedido;
            }
        }
    }
}
