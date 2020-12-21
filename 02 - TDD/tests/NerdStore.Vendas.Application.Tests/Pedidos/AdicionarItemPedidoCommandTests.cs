using FluentAssertions;
using NerdStore.Vendas.Application.Commands;
using NerdStore.Vendas.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NerdStore.Vendas.Application.Tests.Pedidos
{
    public class AdicionarItemPedidoCommandTests
    {
        [Fact(DisplayName = "Adicionar Item Command Válido")]
        [Trait("Categoria", "Vendas - Pedido Commands")]
        public void AdicionarItemPedidoCommand_CommandoEstaValido_DevePassarNaValidacao()
        {
            // Arrange
            var pedidoCommand = new AdicionarItemPedidoCommand(clienteId: Guid.NewGuid(),
                produtoId: Guid.NewGuid(),
                nome: "Produto Teste",
                quantidade: 2,
                valorUnitario: 100);

            // Act
            var result = pedidoCommand.EhValido();

            // Assert
            result.Should().BeTrue();
        }

        [Fact(DisplayName = "Adicionar Item Command Inválido")]
        [Trait("Categoria", "Vendas - Pedido Commands")]
        public void AdicionarItemPedidoCommand_CommandoEstaInvalido_NaoDevePassarNaValidacao()
        {
            // Arrange
            var pedidoCommand = new AdicionarItemPedidoCommand(clienteId: Guid.Empty,
                produtoId: Guid.Empty,
                nome: "",
                quantidade: 0,
                valorUnitario: 0);

            // Act
            var result = pedidoCommand.EhValido();

            // Assert
            result.Should().BeFalse();
            pedidoCommand.ValidationResult.Errors.Should().HaveCount(5);
            pedidoCommand.ValidationResult.Errors.Should().Contain(x => x.ErrorMessage == AdicionarItemPedidoValidation.IdProdutoErroMsg);
            pedidoCommand.ValidationResult.Errors.Should().Contain(x => x.ErrorMessage == AdicionarItemPedidoValidation.IdClienteErroMsg);
            pedidoCommand.ValidationResult.Errors.Should().Contain(x => x.ErrorMessage == AdicionarItemPedidoValidation.NomeErroMsg);
            pedidoCommand.ValidationResult.Errors.Should().Contain(x => x.ErrorMessage == AdicionarItemPedidoValidation.QtdMinErroMsg);
            pedidoCommand.ValidationResult.Errors.Should().Contain(x => x.ErrorMessage == AdicionarItemPedidoValidation.ValorErroMsg);
        }

        [Fact(DisplayName = "Adicionar Item Command Válido")]
        [Trait("Categoria", "Vendas - Pedido Commands")]
        public void AdicionarItemPedidoCommand_QuantidadeUnidadesSuperiorAoPermitido_NaoDevePassarNaValidacao()
        {
            // Arrange
            var pedidoCommand = new AdicionarItemPedidoCommand(clienteId: Guid.NewGuid(),
                produtoId: Guid.NewGuid(),
                nome: "Produto Teste",
                quantidade: Pedido.MAX_UNIDADES_ITEM + 1,
                valorUnitario: 100);

            // Act
            var result = pedidoCommand.EhValido();

            // Assert
            result.Should().BeFalse();
            pedidoCommand.ValidationResult.Errors.Should().HaveCount(1);
            pedidoCommand.ValidationResult.Errors.Should().Contain(x => x.ErrorMessage == AdicionarItemPedidoValidation.QtdMaxErroMsg);
        }
    }
}
