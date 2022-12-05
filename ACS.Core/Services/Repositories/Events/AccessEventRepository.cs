using ACS.Core.Data;
using ACS.Core.Models.Events;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ACS.Core.Services.Repositories.Events
{
    public class AccessEventRepository : GenericRepositoryAsync<AccessEvent>
    {
        public AccessEventRepository(IDbContextFactory<AccessControlDbContext> context) : base(context)
        {
        }

        public override async Task<IEnumerable<AccessEvent>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbSet.Include(entry => entry.AccessPoint)
                .Include(entry => entry.User)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public override async Task<AccessEvent> GetOneAsync(Guid id, CancellationToken cancellationToken)
        {
            var entry = _dbSet.Find(new object[] { id });
            if (entry == null)
                return null;

            await Context.Entry(entry)
                 .Reference(e => e.AccessPoint)
                 .LoadAsync(cancellationToken);

            await Context.Entry(entry)
                 .Reference(e => e.User)
                 .LoadAsync(cancellationToken);

            return entry;
        }

        public override void Update(AccessEvent otherItem)
        {
            return;
        }

        public override void Delete(AccessEvent item)
        {
            return;
        }

        public override Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
