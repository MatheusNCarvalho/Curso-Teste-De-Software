using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NerdStore.Vendas.Domain
{
    public class Pedido
    {
        public decimal ValorTotal { get; private set; }
        public IList<PedidoItem> PedidoItems { get; set; } = new List<PedidoItem>();

        public void AdicionarItem(PedidoItem pedidoItem)
        {
            PedidoItems.Add(pedidoItem);

            ValorTotal = PedidoItems.Sum(i => i.ValorTotal());
        }
    }
}
