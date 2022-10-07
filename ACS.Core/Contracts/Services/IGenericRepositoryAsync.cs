using ACS.Core.Data;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ACS.Core.Contracts.Services
{
    public interface IGenericRepositoryAsync<TEntity> where TEntity : class, IHasGuid
    {
        EntityEntry<TEntity> Attach(TEntity item);
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken);
        Task<TEntity> GetOneAsync(Guid id, CancellationToken cancellationToken);
        Task CreateAsync(TEntity item, CancellationToken cancellationToken);
        void Update(TEntity otherItem);
        void Delete(TEntity item);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
        Task<bool> ElementExistsAsync(Guid id, CancellationToken cancellationToken);
        AccessControlDbContext Context { get; }
    }
}
