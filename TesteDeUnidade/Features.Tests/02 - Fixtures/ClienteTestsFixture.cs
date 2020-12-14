using Bogus;
using Bogus.DataSets;
using Features.Clientes;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Features.Tests
{
    [CollectionDefinition(nameof(ClienteCollection))]
    public class ClienteCollection : ICollectionFixture<ClienteTestsFixture>
    {

    }

    public class ClienteTestsFixture : IDisposable
    {

        public Cliente GerarClienteValido()
        {
            var genero = new Faker().PickRandom<Name.Gender>();

            var cliente = new Faker<Cliente>("pt_BR")
                .CustomInstantiator(f => new Cliente(Guid.NewGuid(),
                    f.Name.FirstName(genero),
                    f.Name.LastName(genero),
                    f.Date.Past(80, DateTime.Now.AddYears(-18)),
                    "",
                    true,
                    DateTime.Now))
                .RuleFor(c => c.Email, (f, c) =>
                    f.Internet.Email(c.Nome.ToLower(), c.Sobrenome.ToLower()));

            return cliente;
        }

        public Cliente GerarClienteInvalido()
        {
            var email = new Faker().Internet.Email();

            var cliente = new Cliente(Guid.NewGuid(),
              "",
              "",
              DateTime.Now.AddYears(-26),
              email,
              true,
              DateTime.Now);

            return cliente;
        }

        public void Dispose()
        {

        }
    }
}
