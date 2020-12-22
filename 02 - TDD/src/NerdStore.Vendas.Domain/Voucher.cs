using System;
using FluentValidation.Results;
using NerdStore.Core.DomainObjects;

namespace NerdStore.Vendas.Domain
{
    public class Voucher : Entity
    {
        protected Voucher() { }

        public Voucher(TipoDescontoVoucher tipoDescontoVoucher,
            string codigo,
            decimal? valorDesconto,
            decimal? percentualDesconto,
            int quantidade,
            DateTime dataValidade,
            bool ativo,
            bool utilizado)
        {
            TipoDescontoVoucher = tipoDescontoVoucher;
            Codigo = codigo;
            ValorDesconto = valorDesconto;
            PercentualDesconto = percentualDesconto;
            Quantidade = quantidade;
            DataValidade = dataValidade;
            Ativo = ativo;
            Utilizado = utilizado;
        }

        public TipoDescontoVoucher TipoDescontoVoucher { get; private set; }
        public string Codigo { get; private set; }
        public decimal? ValorDesconto { get; private set; }
        public decimal? PercentualDesconto { get; private set; }
        public int Quantidade { get; private set; }
        public DateTime DataValidade { get; private set; }
        public bool Ativo { get; private set; }
        public bool Utilizado { get; private set; }

        public ValidationResult ValidarSeAplicavel()
        {
            return new VoucherAplicavelValidation().Validate(this);
        }
    }
}
