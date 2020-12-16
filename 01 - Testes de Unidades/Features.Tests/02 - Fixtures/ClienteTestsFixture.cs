using Bogus;
using Bogus.DataSets;
using Features.Clientes;
using System;
using System.Collections.Generic;
using System.Linq;
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
            return GerarClientes(1).FirstOrDefault();
        }

        public IList<Cliente> GerarClientesAtivosAndInativos()
        {
            var clientes = new List<Cliente>();
            clientes.AddRange(GerarClientes(50));
            clientes.AddRange(GerarClientes(50, false));

            return clientes;
        }


        public IList<Cliente> GerarClientes(int quantidade, bool ativo = true)
        {
            var genero = new Faker().PickRandom<Name.Gender>();

            var clientes = new Faker<Cliente>("pt_BR")
                .CustomInstantiator(f => new Cliente(Guid.NewGuid(),
                    f.Name.FirstName(genero),
                    f.Name.LastName(genero),
                    f.Date.Past(80, DateTime.Now.AddYears(-18)),
                    "",
                    ativo,
                    DateTime.Now))
                .RuleFor(c => c.Email, (f, c) =>
                    f.Internet.Email(c.Nome.ToLower(), c.Sobrenome.ToLower()));

            return clientes.Generate(quantidade);
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
