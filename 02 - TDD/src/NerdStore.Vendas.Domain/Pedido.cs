using FluentValidation.Results;
using NerdStore.Core.DomainObjects;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public Voucher Voucher { get; private set; }
        public bool VoucherUtilizado { get; private set; }
        public decimal ValorDesconto { get; private set; }
        public void TornarRascunho()
        {
            PedidoStatus = PedidoStatus.Rascunho;
        }

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

        public void AtualizarItem(PedidoItem pedidoItem)
        {
            ValidarPedidoItemInexistente(pedidoItem);
            ValidarQuantidadeItemPermitida(pedidoItem);

            var itemExistente = PedidoItems.FirstOrDefault(p => p.ProdutoId == pedidoItem.ProdutoId);

            PedidoItems.Remove(itemExistente);
            PedidoItems.Add(pedidoItem);

            CalcularValorPedido();
        }

        public void RemoverItem(PedidoItem pedidoItem)
        {
            ValidarPedidoItemInexistente(pedidoItem);

            PedidoItems.Remove(pedidoItem);

            CalcularValorPedido();
        }

        public ValidationResult AplicarVoucher(Voucher voucher)
        {
            var result = voucher.ValidarSeAplicavel();
            if (!result.IsValid)
            {
                return result;
            }

            Voucher = voucher;
            VoucherUtilizado = true;

            CalcularValorTotalDesconto();

            return result;
        }

        private void CalcularValorTotalDesconto()
        {
            if (!VoucherUtilizado)
            {
                return;
            }

            decimal desconto = 0;
            var valor = ValorTotal;

            if (Voucher.TipoDescontoVoucher == TipoDescontoVoucher.Valor)
            {
                if (Voucher.ValorDesconto.HasValue)
                {
                    desconto = Voucher.ValorDesconto.Value;
                    valor -= desconto;
                }
            }
            else
            {
                if (Voucher.PercentualDesconto.HasValue)
                {
                    desconto = (ValorTotal * Voucher.PercentualDesconto.Value) / 100;
                    valor -= desconto;
                }
            }


            ValorTotal = valor < 0 ? 0 : valor;
            ValorDesconto = desconto;
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

        private void ValidarPedidoItemInexistente(PedidoItem item)
        {
            if (!PedidoItemExistente(item))
            {
                throw new DomainExeciption("O item não existe no pedido");
            }
        }

        private void CalcularValorPedido()
        {
            ValorTotal = PedidoItems.Sum(i => i.CalcularValor());
            CalcularValorTotalDesconto();
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
