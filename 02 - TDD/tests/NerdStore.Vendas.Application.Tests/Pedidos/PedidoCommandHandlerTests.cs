using FluentAssertions;
using MediatR;
using NerdStore.Core.Data;
using NerdStore.Vendas.Application.Commands;
using NerdStore.Vendas.Domain;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace NerdStore.Vendas.Application.Tests.Pedidos
{
    public class PedidoCommandHandlerTests
    {

        [Fact(DisplayName = "Adicionar Item Novo Pedido com Sucesso")]
        [Trait("Categoria", "Vendas - Pedido Command Handler")]
        public async Task AdicionarItem_NovoPedido_DeveExecutarComSucesso()
        {
            // Arrange
            var pedidoCommand = new AdicionarItemPedidoCommand(clienteId: Guid.NewGuid(),
             produtoId: Guid.NewGuid(),
             nome: "Produto Teste",
             quantidade: 2,
             valorUnitario: 100);

            // Act         
            var pedidoRepository = Substitute.For<IPedidoRepository>();
            var mediator = Substitute.For<IMediator>();
            var pedidoHandler = Substitute.For<PedidoCommandHandler>(pedidoRepository, mediator);

            pedidoRepository.UnitOfWork.Commit().Returns(true);

            var result = await pedidoHandler.Handle(pedidoCommand, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            pedidoRepository.Received(1).Adicionar(Arg.Any<Pedido>());
            await pedidoRepository.UnitOfWork.Received(1).Commit();
        }

        [Fact(DisplayName = "Adicionar Item Command Inválido")]
        [Trait("Categoria", "Vendas - Pedido Command Handler")]
        public async Task AdicionarItem_CommandoInvalido_DeveRetornarFalsoELancarEventosDeNotificacao()
        {
            // Arrange
            var pedidoCommand = new AdicionarItemPedidoCommand(clienteId: Guid.Empty,
             produtoId: Guid.NewGuid(),
             nome: "",
             quantidade: 0,
             valorUnitario: 0);

            // Act         
            var pedidoRepository = Substitute.For<IPedidoRepository>();
            var mediator = Substitute.For<IMediator>();
            var pedidoHandler = Substitute.For<PedidoCommandHandler>(pedidoRepository, mediator);   

            var result = await pedidoHandler.Handle(pedidoCommand, CancellationToken.None);

            // Assert
            result.Should().BeFalse();   
            await mediator.Received(4).Publish(Arg.Any<INotification>());
        }

        [Fact(DisplayName = "Adicionar Item Novo Pedido Rascunho com Sucesso")]
        [Trait("Categoria", "Vendas - Pedido Command Handler")]
        public async Task AdicionarItem_NovoItemAoPedidoRascunho_DeveExecutarComSucesso()
        {
            // Arrange
            var clienteId = Guid.NewGuid();

            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(clienteId);
            var pedidoItemExistente = new PedidoItem(Guid.NewGuid(), "Produto 01", 2, 100);
            pedido.AdicionarItem(pedidoItemExistente);

            var pedidoCommand = new AdicionarItemPedidoCommand(clienteId: clienteId,
             produtoId: Guid.NewGuid(),
             nome: "Produto Teste",
             quantidade: 2,
             valorUnitario: 100);

            // Act         
            var pedidoRepository = Substitute.For<IPedidoRepository>();
            var mediator = Substitute.For<IMediator>();
            var pedidoHandler = Substitute.For<PedidoCommandHandler>(pedidoRepository, mediator);

            pedidoRepository.ObterPedidoRascunhoPorClienteId(clienteId).Returns(pedido);
            pedidoRepository.UnitOfWork.Commit().Returns(true);

            var result = await pedidoHandler.Handle(pedidoCommand, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            pedidoRepository.Received(1).AdicionarItem(Arg.Any<PedidoItem>());
            pedidoRepository.Received(1).Atualizar(Arg.Any<Pedido>());
            await pedidoRepository.UnitOfWork.Received(1).Commit();
        }

        [Fact(DisplayName = "Adicionar Item Existente ao Pedido Rascunho com Sucesso")]
        [Trait("Categoria", "Vendas - Pedido Command Handler")]
        public async Task AdicionarItem_ItemExistenteAoPedidoRascunho_DeveExecutarComSucesso()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var produtoId = Guid.NewGuid();

            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(clienteId);
            var pedidoItemExistente = new PedidoItem(produtoId, "Produto 01", 2, 100);
            pedido.AdicionarItem(pedidoItemExistente);

            var pedidoCommand = new AdicionarItemPedidoCommand(clienteId: clienteId, produtoId: produtoId, nome: "Produto 01",
             quantidade: 2, valorUnitario: 100);

            // Act         
            var pedidoRepository = Substitute.For<IPedidoRepository>();
            var mediator = Substitute.For<IMediator>();
            var pedidoHandler = Substitute.For<PedidoCommandHandler>(pedidoRepository, mediator);

            pedidoRepository.ObterPedidoRascunhoPorClienteId(clienteId).Returns(pedido);
            pedidoRepository.UnitOfWork.Commit().Returns(true);

            var result = await pedidoHandler.Handle(pedidoCommand, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            pedidoRepository.Received(1).AtualizarItem(Arg.Any<PedidoItem>());
            pedidoRepository.Received(1).Atualizar(Arg.Any<Pedido>());
            await pedidoRepository.UnitOfWork.Received(1).Commit();
        }

    }
}
