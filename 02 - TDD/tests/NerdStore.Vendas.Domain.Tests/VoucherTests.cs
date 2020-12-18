using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NerdStore.Vendas.Domain.Tests
{
    public class VoucherTests
    {
        [Fact(DisplayName = "Validar Voucher Tipo Valor Válido")]
        [Trait("Categoria", "Vendas - Voucher")]
        public void Voucher_ValidarVoucherTipoValor_DeveEstarValido()
        {
            // Arrange
            var voucher = new Voucher(TipoDescontoVoucher.Valor, "PROMO-15-REAIS", 15, null, 1, DateTime.Now.AddDays(15), true, false);

            // Act
            var result = voucher.ValidarSeAplicavel();

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact(DisplayName = "Validar Voucher Tipo Valor Inválido")]
        [Trait("Categoria", "Vendas - Voucher")]
        public void Voucher_ValidarVoucherTipoValor_DeveEstarInvalido()
        {
            // Arrange
            var voucher = new Voucher(TipoDescontoVoucher.Valor, "", null, null, 0, DateTime.Now.AddDays(-1), false, true);

            // Act
            var result = voucher.ValidarSeAplicavel();

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(6);
            result.Errors.Should().Contain(x => x.ErrorMessage == VoucherAplicavelValidation.AtivoErroMsg);
            result.Errors.Should().Contain(x => x.ErrorMessage == VoucherAplicavelValidation.CodigoErroMsg);
            result.Errors.Should().Contain(x => x.ErrorMessage == VoucherAplicavelValidation.DataValidadeErroMsg);
            result.Errors.Should().Contain(x => x.ErrorMessage == VoucherAplicavelValidation.QuantidadeErroMsg);
            result.Errors.Should().Contain(x => x.ErrorMessage == VoucherAplicavelValidation.UtilizadoErroMsg);
            result.Errors.Should().Contain(x => x.ErrorMessage == VoucherAplicavelValidation.ValorDescontoErroMsg);
        }

        [Fact(DisplayName = "Validar Voucher Porcentagem Válido")]
        [Trait("Categoria", "Vendas - Voucher")]
        public void Voucher_ValidarVoucherPorcentagem_DeveEstarValido()
        {
            // Arrange           
            var voucher = new Voucher(tipoDescontoVoucher: TipoDescontoVoucher.Valor,
                codigo: "PROMO-15-REAIS",
                valorDesconto: 15,
                percentualDesconto: null,
                quantidade: 1,
                dataValidade: DateTime.Now.AddDays(15),
                ativo: true,
                utilizado: false);

            // Act
            var result = voucher.ValidarSeAplicavel();

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact(DisplayName = "Validar Voucher Porcentagem Inválido")]
        [Trait("Categoria", "Vendas - Voucher")]
        public void Voucher_ValidarVoucherPorcentagem_DeveEstarInvalido()
        {
            // Arrange           
            var voucher = new Voucher(tipoDescontoVoucher: TipoDescontoVoucher.Porcentagem,
                codigo: "",
                valorDesconto: null,
                percentualDesconto: null,
                quantidade: 0,
                dataValidade: DateTime.Now.AddDays(-1),
                ativo: false,
                utilizado: true);

            // Act
            var result = voucher.ValidarSeAplicavel();

            // Assert
            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(6);
            result.Errors.Should().Contain(x => x.ErrorMessage == VoucherAplicavelValidation.AtivoErroMsg);
            result.Errors.Should().Contain(x => x.ErrorMessage == VoucherAplicavelValidation.CodigoErroMsg);
            result.Errors.Should().Contain(x => x.ErrorMessage == VoucherAplicavelValidation.DataValidadeErroMsg);
            result.Errors.Should().Contain(x => x.ErrorMessage == VoucherAplicavelValidation.QuantidadeErroMsg);
            result.Errors.Should().Contain(x => x.ErrorMessage == VoucherAplicavelValidation.UtilizadoErroMsg);
            result.Errors.Should().Contain(x => x.ErrorMessage == VoucherAplicavelValidation.PercentualDescontoErroMsg);
        }
    }
}
