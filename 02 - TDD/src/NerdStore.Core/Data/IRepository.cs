using NerdStore.Core.DomainObjects;
using System;

namespace NerdStore.Core.Data
{
    public interface IRepository<TEntity> : IDisposable where TEntity : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
    }  
}
