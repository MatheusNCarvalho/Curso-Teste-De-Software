﻿using Features.Clientes;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Features.Tests
{
    public class ClienteTests
    {

        [Fact(DisplayName = "Novo Cliente Válido")]
        [Trait("Categoria","Cliente Trait Testes")]
        public void Cliente_NovoCliente_DeveEstarValido()
        {
            //Arrange 
            var cliente = new Cliente(Guid.NewGuid(),
                "Matheus", "Neves",
                DateTime.Now.AddYears(-26),
                "matheus@gmail.com",
                true,
                DateTime.Now);

            //Act
            var result = cliente.EhValido();

            //Assert
            Assert.True(result);
            Assert.Equal(0, cliente.ValidationResult.Errors.Count);
        }

        [Fact(DisplayName = "Novo Cliente Inválido")]
        [Trait("Categoria", "Cliente Trait Testes")]
        public void Cliente_NovoCliente_DeveEstarInvalido()
        {
            //Arrange 
            var cliente = new Cliente(Guid.NewGuid(),
                "", 
                "",
                DateTime.Now.AddYears(-26),
                "matheus@gmail.com",
                true,
                DateTime.Now);

            //Act
            var result = cliente.EhValido();

            //Assert
            Assert.False(result);
            Assert.NotEqual(0, cliente.ValidationResult.Errors.Count);
        }
    }
}