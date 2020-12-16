using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Features.Tests
{
    [Collection(nameof(ClienteCollection))]
    public class ClienteFluentAssertionsTests
    {
        private readonly ClienteTestsFixture _clienteTestsFixture;
        private readonly ITestOutputHelper _testOutputHelper;

        public ClienteFluentAssertionsTests(ClienteTestsFixture clienteTestsFixture, ITestOutputHelper testOutputHelper)
        {
            _clienteTestsFixture = clienteTestsFixture;
            _testOutputHelper = testOutputHelper;
        }


        [Fact(DisplayName = "Novo Cliente Válido")]
        [Trait("Categoria", "Cliente Fluent Assertions Testes")]
        public void Cliente_NovoCliente_DeveEstarValido()
        {
            //Arrange 
            var cliente = _clienteTestsFixture.GerarClienteValido();

            //Act
            var result = cliente.EhValido();

            //Assert
            //Assert.True(result);
            //Assert.Equal(0, cliente.ValidationResult.Errors.Count);

            //Assert Fluent
            result.Should().BeTrue();
            cliente.ValidationResult.Errors.Should().HaveCount(0);
        }

        [Fact(DisplayName = "Novo Cliente Inválido")]
        [Trait("Categoria", "Cliente Fluent Assertions Testes")]
        public void Cliente_NovoCliente_DeveEstarInvalido()
        {
            //Arrange 
            var cliente = _clienteTestsFixture.GerarClienteInvalido();

            //Act
            var result = cliente.EhValido();

            //Assert
            //Assert.False(result);
            //Assert.NotEqual(0, cliente.ValidationResult.Errors.Count);

            //Assert Fluent
            result.Should().BeFalse();
            cliente.ValidationResult.Errors.Should().HaveCountGreaterOrEqualTo(1, "deve possuir erros de validações");

            _testOutputHelper.WriteLine($"Foram encontrados {cliente.ValidationResult.Errors.Count} erros nessa validação");
        }
    }
}
