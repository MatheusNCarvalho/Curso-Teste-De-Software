using MediatR;
using NerdStore.Core.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace NerdStore.Vendas.Application.Events
{
    public class PedidoItemAdicionadoEvent : Event
    {
        public PedidoItemAdicionadoEvent(Guid clienteId, Guid produtoId, int quantidade, decimal valorUnitario, Guid pedidoId, string produtoNome)
        {
            ClienteId = clienteId;
            ProdutoId = produtoId;
            Quantidade = quantidade;
            ValorUnitario = valorUnitario;
            PedidoId = pedidoId;
            ProdutoNome = produtoNome;
        }

        public Guid ClienteId { get; private set; }
        public Guid ProdutoId { get; private set; }
        public Guid PedidoId { get; private set; }
        public string ProdutoNome { get; private set; }
        public int Quantidade { get; private set; }
        public decimal ValorUnitario { get; private set; }
    }
}
