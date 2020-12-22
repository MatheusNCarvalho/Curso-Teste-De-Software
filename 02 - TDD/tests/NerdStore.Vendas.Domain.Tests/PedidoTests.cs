using FluentAssertions;
using NerdStore.Core.DomainObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace NerdStore.Vendas.Domain.Tests
{
    public class PedidoTests
    {

        [Fact(DisplayName = "Adicionar Item Novo Pedido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AdicionarItemPedido_NovoPedido_DeveAtualizarValor()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var pedidoItem = new PedidoItem(Guid.NewGuid(), "Produto Teste", 2, 100);

            // Act
            pedido.AdicionarItem(pedidoItem);

            // Assert
            pedido.ValorTotal.Should().Be(200);
        }

        [Fact(DisplayName = "Adicionar Item Pedido Existente")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AdicionarItemPedido_ItemExistente_DeveIncrementarUnidadesSomarValores()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var pedidoItem = new PedidoItem(produtoId, "Produto Teste", 2, 100);
            pedido.AdicionarItem(pedidoItem);

            var pedidoItem2 = new PedidoItem(produtoId, "Produto Teste", 1, 100);

            // Act
            pedido.AdicionarItem(pedidoItem2);

            // Assert
            pedido.ValorTotal.Should().Be(300);
            pedido.PedidoItems.Should().HaveCount(1);
            pedido.PedidoItems.FirstOrDefault(p => p.ProdutoId == produtoId).Quantidade.Should().Be(3);
        }

        [Fact(DisplayName = "Adicionar Item Pedido acima do permitido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AdicionarItemPedido_UnidadesItemAcimaDoPermitido_DeveRetornarException()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var pedidoItem = new PedidoItem(produtoId, "Produto Teste", Pedido.MAX_UNIDADES_ITEM + 1, 100);

            // Act & Assert
            Action action = () => pedido.AdicionarItem(pedidoItem);

            action.Should().Throw<DomainException>();
        }

        [Fact(DisplayName = "Adicionar Item Pedido Existente acima do permitido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AdicionarItemPedido_ItemExistenteSomaUnidadesAcimaDoPermitido_DeveRetornarException()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var pedidoItem = new PedidoItem(produtoId, "Produto Teste", Pedido.MAX_UNIDADES_ITEM, 100);
            var pedidoItem2 = new PedidoItem(produtoId, "Produto Teste", 2, 100);
            pedido.AdicionarItem(pedidoItem);

            // Act & Assert
            Action action = () => pedido.AdicionarItem(pedidoItem2);

            action.Should().Throw<DomainException>();
        }

        [Fact(DisplayName = "Atualizar Item Pedido Inexistente")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AtualizarItemPedido_ItemNaoExisteNaLista_DeveRetornarException()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var peditoItemAtualizado = new PedidoItem(Guid.NewGuid(), "Produto Teste", 5, 100);

            // Act &  Assert
            Action action = () => pedido.AtualizarItem(peditoItemAtualizado);
            action.Should().Throw<DomainException>();
        }

        [Fact(DisplayName = "Atualizar Item Pedido Valido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AtualizarItemPedido_ItemValido_DeveAtualizarQuantidade()
        {
            // Arrange            
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var peditoItem = new PedidoItem(produtoId, "Produto Teste", 2, 100);
            pedido.AdicionarItem(peditoItem);

            var pedidoItemAtualizado = new PedidoItem(produtoId, "Produto Teste", 5, 100);
            var novaQuantidade = pedidoItemAtualizado.Quantidade;

            // Act
            pedido.AtualizarItem(pedidoItemAtualizado);

            //Assert
            pedido.PedidoItems.FirstOrDefault(p => p.ProdutoId == produtoId).Quantidade.Should().Be(novaQuantidade);
        }

        [Fact(DisplayName = "Atualizar Item Pedido Validar Total")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AtualizarItemPedido_PedidoComProdutoDiferentes_DeveAtualizarValorTotal()
        {
            // Arrange            
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var peditoItemExistente1 = new PedidoItem(Guid.NewGuid(), "Produto Xpto", 2, 100);
            var peditoItemExistente2 = new PedidoItem(produtoId, "Produto Teste", 3, 15);
            pedido.AdicionarItem(peditoItemExistente1);
            pedido.AdicionarItem(peditoItemExistente2);

            var pedidoItemAtualizado = new PedidoItem(produtoId, "Produto Teste", 5, 15);

            var totalPedido = peditoItemExistente1.Quantidade * peditoItemExistente1.ValorUnitario +
                              pedidoItemAtualizado.Quantidade * pedidoItemAtualizado.ValorUnitario;

            // Act
            pedido.AtualizarItem(pedidoItemAtualizado);

            //Assert
            pedido.ValorTotal.Should().Be(totalPedido);
        }

        [Fact(DisplayName = "Atualizar Item Pedido Quantidade acima do permitido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AtualizarItemPedido_ItemUnidadesAcimaDoPermitido_DeveRetornarException()
        {
            // Arrange            
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var peditoItemExistente1 = new PedidoItem(produtoId, "Produto Xpto", 3, 15);
            pedido.AdicionarItem(peditoItemExistente1);

            var pedidoItemAtualizado = new PedidoItem(produtoId, "Produto Teste", Pedido.MAX_UNIDADES_ITEM + 1, 15);

            // Act & Assert
            Action action = () => pedido.AtualizarItem(pedidoItemAtualizado);
            action.Should().Throw<DomainException>();
        }

        [Fact(DisplayName = "Remover Item Pedido Inexistente")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void RemoverItemPedido_ItemNaoExisteNaLista_DeveRetornarException()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var peditoItemRemover = new PedidoItem(produtoId, "Produto Xpto", 3, 15);

            // Act & Assert
            Action action = () => pedido.RemoverItem(peditoItemRemover);
            action.Should().Throw<DomainException>();
        }

        [Fact(DisplayName = "Remover Item Pedido Deve Calcular Valor Total")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void RemoverItemPedido_ItemExistente_DeveAtualizarValorTotal()
        {
            // Arrange            
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var peditoItem1 = new PedidoItem(Guid.NewGuid(), "Produto Xpto", 2, 100);
            var peditoItem2 = new PedidoItem(produtoId, "Produto Teste", 3, 15);
            pedido.AdicionarItem(peditoItem1);
            pedido.AdicionarItem(peditoItem2);

            var totalPedido = peditoItem1.Quantidade * peditoItem1.ValorUnitario;

            // Act
            pedido.RemoverItem(peditoItem2);

            //Assert
            pedido.ValorTotal.Should().Be(totalPedido);
        }

        [Fact(DisplayName = "Aplicar Voucher Válido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void Pedido_AplicarVoucherValido_DeveRetornarSemErros()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var voucher = new Voucher(tipoDescontoVoucher: TipoDescontoVoucher.Valor,
                          codigo: "PROMO-15-REAIS",
                          valorDesconto: 15,
                          percentualDesconto: null,
                          quantidade: 1,
                          dataValidade: DateTime.Now.AddDays(15),
                          ativo: true,
                          utilizado: false);

            // Act
            var result = pedido.AplicarVoucher(voucher);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact(DisplayName = "Aplicar Voucher Inválido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void Pedido_AplicarVoucherValido_DeveRetornarComErros()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var voucher = new Voucher(tipoDescontoVoucher: TipoDescontoVoucher.Valor,
                           codigo: "",
                           valorDesconto: null,
                           percentualDesconto: null,
                           quantidade: 0,
                           dataValidade: DateTime.Now.AddDays(15),
                           ativo: true,
                           utilizado: false);

            // Act
            var result = pedido.AplicarVoucher(voucher);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact(DisplayName = "Aplicar Voucher Tipo Valor Desconto")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AplicarVoucher_VoucherTipoValorDesconto_DeveDescontarDoValorTotal()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var peditoItem1 = new PedidoItem(Guid.NewGuid(), "Produto Xpto", 2, 100);
            var peditoItem2 = new PedidoItem(produtoId, "Produto Teste", 3, 15);
            pedido.AdicionarItem(peditoItem1);
            pedido.AdicionarItem(peditoItem2);

            // Act
            var voucher = new Voucher(tipoDescontoVoucher: TipoDescontoVoucher.Valor,
                          codigo: "PROMO-15-REAIS",
                          valorDesconto: 15,
                          percentualDesconto: null,
                          quantidade: 1,
                          dataValidade: DateTime.Now.AddDays(15),
                          ativo: true,
                          utilizado: false);

            var valorComDesconto = pedido.ValorTotal - voucher.ValorDesconto;
            pedido.AplicarVoucher(voucher);

            // Assert
            valorComDesconto.Should().Be(pedido.ValorTotal);
        }

        [Fact(DisplayName = "Aplicar Voucher Tipo Percentual Desconto")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AplicarVoucher_VoucherTipoPercentualDesconto_DeveDescontarDoValorTotal()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var peditoItem1 = new PedidoItem(Guid.NewGuid(), "Produto Xpto", 2, 100);
            var peditoItem2 = new PedidoItem(produtoId, "Produto Teste", 3, 15);
            pedido.AdicionarItem(peditoItem1);
            pedido.AdicionarItem(peditoItem2);

            // Act
            var voucher = new Voucher(tipoDescontoVoucher: TipoDescontoVoucher.Porcentagem,
                          codigo: "PROMO-15-REAIS",
                          valorDesconto: null,
                          percentualDesconto: 15,
                          quantidade: 1,
                          dataValidade: DateTime.Now.AddDays(15),
                          ativo: true,
                          utilizado: false);

            var valorDesconto = (pedido.ValorTotal * voucher.PercentualDesconto) / 100;
            var valorTotalComDesconto = pedido.ValorTotal - valorDesconto;
            pedido.AplicarVoucher(voucher);

            // Assert
            valorTotalComDesconto.Should().Be(pedido.ValorTotal);
        }

        [Fact(DisplayName = "Aplicar Vouche Desconto Excede Valor Total")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AplicarVoucher_DescontoExcedeValorTotalPedido_PedidoDeveTerValorZero()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            var peditoItem = new PedidoItem(Guid.NewGuid(), "Produto Xpto", 2, 100);
            pedido.AdicionarItem(peditoItem);

            // Act
            var voucher = new Voucher(tipoDescontoVoucher: TipoDescontoVoucher.Valor,
                          codigo: "PROMO-15-REAIS",
                          valorDesconto: 300,
                          percentualDesconto: null,
                          quantidade: 1,
                          dataValidade: DateTime.Now.AddDays(15),
                          ativo: true,
                          utilizado: false);

            pedido.AplicarVoucher(voucher);

            // Assert
            pedido.ValorTotal.Should().Be(0);
        }


        [Fact(DisplayName = "Aplicar Voucher Recalcular Desconto na Modificação do pedido")]
        [Trait("Categoria", "Vendas - Pedido")]
        public void AplicarVoucher_ModificarItensPedido_DeveCalcularDescontoValorTotal()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());

            var peditoItem = new PedidoItem(Guid.NewGuid(), "Produto Xpto", 2, 100);
            pedido.AdicionarItem(peditoItem);

            var voucher = new Voucher(tipoDescontoVoucher: TipoDescontoVoucher.Valor,
                          codigo: "PROMO-15-REAIS",
                          valorDesconto: 300,
                          percentualDesconto: null,
                          quantidade: 1,
                          dataValidade: DateTime.Now.AddDays(15),
                          ativo: true,
                          utilizado: false);

            pedido.AplicarVoucher(voucher);

            var peditoItem2 = new PedidoItem(Guid.NewGuid(), "Produto Xpto", 2, 100);

            //Act
            pedido.AdicionarItem(peditoItem2);

            // Assert
            var totalEsperado = pedido.PedidoItems.Sum(x => x.CalcularValor()) - voucher.ValorDesconto;

            totalEsperado.Should().Be(pedido.ValorTotal);
        }
    }
}
