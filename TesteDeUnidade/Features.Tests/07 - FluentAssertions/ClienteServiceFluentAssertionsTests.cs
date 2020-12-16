﻿using Features.Clientes;
using FluentAssertions;
using FluentAssertions.Extensions;
using MediatR;
using Moq;
using Moq.AutoMock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit;

namespace Features.Tests
{
    [Collection(nameof(ClienteCollection))]
    public class ClienteServiceFluentAssertionsTests
    {
        private readonly ClienteTestsFixture _clienteFixtureTests;

        public ClienteServiceFluentAssertionsTests(ClienteTestsFixture clienteFixtureTests)
        {
            _clienteFixtureTests = clienteFixtureTests;
        }

        [Fact(DisplayName = "Adicionar Cliente com sucesso")]
        [Trait("Categoria", "Cliente Service Fluent Assertions Tests")]
        public void ClienteService_Adicionar_DeveExecutarComSucesso()
        {
            //Arrange 
            var cliente = _clienteFixtureTests.GerarClienteValido();
            var mocker = new AutoMocker();
            var clienteService = mocker.CreateInstance<ClienteService>();

            //Act
            clienteService.Adicionar(cliente);

            //Assert
            Assert.True(cliente.EhValido());
            mocker.GetMock<IClienteRepository>().Verify(r => r.Adicionar(cliente), Times.Once);
            mocker.GetMock<IMediator>().Verify(m => m.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Once);
        }

        [Fact(DisplayName = "Adicionar Cliente com falha")]
        [Trait("Categoria", "Cliente Service Fluent Assertions Tests")]
        public void ClienteService_Adicionar_DeveFalharDevidoClienteInvalido()
        {
            //Arrange 
            var cliente = _clienteFixtureTests.GerarClienteInvalido();
            var mocker = new AutoMocker();
            var clienteService = mocker.CreateInstance<ClienteService>();

            //Act
            clienteService.Adicionar(cliente);

            //Assert           
            mocker.GetMock<IClienteRepository>().Verify(r => r.Adicionar(cliente), Times.Never);
            mocker.GetMock<IMediator>().Verify(m => m.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Never);
        }

        [Fact(DisplayName = "Obter Clientes Ativos")]
        [Trait("Categoria", "Cliente Service Fluent Assertions Tests")]
        public void ClienteService_ObterTodosAtivos_DeveRetornarApenasClientesAtivos()
        {
            //Arrange
            var mocker = new AutoMocker();
            var clienteService = mocker.CreateInstance<ClienteService>();

            mocker.GetMock<IClienteRepository>().Setup(c => c.ObterTodos())
                .Returns(_clienteFixtureTests.GerarClientesAtivosAndInativos());

            //Act
            var clientes = clienteService.ObterTodosAtivos();

            //Assert
            mocker.GetMock<IClienteRepository>().Verify(r => r.ObterTodos(), Times.Once);        

            clientes.Should().HaveCountGreaterOrEqualTo(1).And.OnlyHaveUniqueItems();
            clientes.Should().NotContain(c => !c.Ativo);

            //clienteService.ExecutionTimeOf(c => c.ObterTodosAtivos())
            //    .Should()
            //    .BeLessOrEqualTo(50.Milliseconds(), "é executado milhares de vezes por segundo");
        }
    }
}
