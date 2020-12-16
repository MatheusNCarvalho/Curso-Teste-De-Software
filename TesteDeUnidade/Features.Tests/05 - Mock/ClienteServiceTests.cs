using Features.Clientes;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit;

namespace Features.Tests
{
    [Collection(nameof(ClienteCollection))]
    public class ClienteServiceTests
    {
        private readonly ClienteTestsFixture _clienteFixtureTests;

        public ClienteServiceTests(ClienteTestsFixture clienteFixtureTests)
        {
            _clienteFixtureTests = clienteFixtureTests;
        }

        [Fact(DisplayName = "Adicionar Cliente com sucesso")]
        [Trait("Categoria", "Cliente Service Mock Tests")]
        public void ClienteService_Adicionar_DeveExecutarComSucesso()
        {
            //Arrange 
            var cliente = _clienteFixtureTests.GerarClienteValido();
            var clienteRepositorio = new Mock<IClienteRepository>();
            var mediator = new Mock<IMediator>();

            var clienteService = new ClienteService(clienteRepositorio.Object, mediator.Object);

            //Act
            clienteService.Adicionar(cliente);

            //Assert
            Assert.True(cliente.EhValido());
            clienteRepositorio.Verify(r => r.Adicionar(cliente), Times.Once);
            mediator.Verify(m => m.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Once);
        }

        [Fact(DisplayName = "Adicionar Cliente com falha")]
        [Trait("Categoria", "Cliente Service Mock Tests")]
        public void ClienteService_Adicionar_DeveFalharDevidoClienteInvalido()
        {
            //Arrange 
            var cliente = _clienteFixtureTests.GerarClienteInvalido();
            var clienteRepositorio = new Mock<IClienteRepository>();
            var mediator = new Mock<IMediator>();

            var clienteService = new ClienteService(clienteRepositorio.Object, mediator.Object);

            //Act
            clienteService.Adicionar(cliente);

            //Assert           
            clienteRepositorio.Verify(r => r.Adicionar(cliente), Times.Never);
            mediator.Verify(m => m.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Never);
        }

        [Fact(DisplayName = "Obter Clientes Ativos")]
        [Trait("Categoria", "Cliente Service Mock Tests")]
        public void ClienteService_ObterTodosAtivos_DeveRetornarApenasClientesAtivos()
        {
            //Arrange
            var clienteRepositorio = new Mock<IClienteRepository>();
            var mediator = new Mock<IMediator>();
            clienteRepositorio.Setup(c => c.ObterTodos())
                .Returns(_clienteFixtureTests.GerarClientesAtivosAndInativos());

            var clienteService = new ClienteService(clienteRepositorio.Object, mediator.Object);

            //Act
            var clientes = clienteService.ObterTodosAtivos();

            //Assert
            clienteRepositorio.Verify(r => r.ObterTodos(), Times.Once);
            Assert.True(clientes.Any());
            Assert.False(clientes.Count(c => !c.Ativo) > 0);
        }

    }
}
