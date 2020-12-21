using NerdStore.Core.DomainObjects;
using System;

namespace NerdStore.Core.Data
{
    public interface IRepository<TEntity> : IDisposable where TEntity : IAggreagteRoot
    {
        IUnitOfWork UnitOfWork { get; }
    }  
}
