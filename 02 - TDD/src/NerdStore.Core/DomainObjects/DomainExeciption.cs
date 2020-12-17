using System;

namespace NerdStore.Core.DomainObjects
{
    public class DomainExeciption : Exception
    {
        public DomainExeciption()
        {
        }

        public DomainExeciption(string message) : base(message)
        {
        }

        public DomainExeciption(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
