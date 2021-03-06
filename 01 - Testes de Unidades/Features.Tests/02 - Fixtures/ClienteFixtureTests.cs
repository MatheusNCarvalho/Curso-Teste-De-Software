﻿using Features.Clientes;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Features.Tests
{
    [Collection(nameof(ClienteCollection))]
    public class ClienteFixtureTests
    {
        private readonly ClienteTestsFixture _clienteTestsFixture;

        public ClienteFixtureTests(ClienteTestsFixture clienteTestsFixture)
        {
            _clienteTestsFixture = clienteTestsFixture;
        }


        [Fact(DisplayName = "Novo Cliente Válido")]
        [Trait("Categoria", "Cliente Features Testes")]
        public void Cliente_NovoCliente_DeveEstarValido()
        {
            //Arrange 
            var cliente = _clienteTestsFixture.GerarClienteValido();

            //Act
            var result = cliente.EhValido();

            //Assert
            Assert.True(result);
            Assert.Equal(0, cliente.ValidationResult.Errors.Count);
        }

        [Fact(DisplayName = "Novo Cliente Inválido")]
        [Trait("Categoria", "Cliente Features Testes")]
        public void Cliente_NovoCliente_DeveEstarInvalido()
        {
            //Arrange 
            var cliente = _clienteTestsFixture.GerarClienteInvalido();

            //Act
            var result = cliente.EhValido();

            //Assert
            Assert.False(result);
            Assert.NotEqual(0, cliente.ValidationResult.Errors.Count);
        }
    }


    
}
