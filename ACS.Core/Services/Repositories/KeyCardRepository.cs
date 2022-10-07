using ACS.Core.Data;
using ACS.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ACS.Core.Services.Repositories
{
    public class KeyCardRepository : GenericRepositoryAsync<KeyCard>
    {
        public KeyCardRepository(AccessControlDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<KeyCard>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbSet.Include(u => u.AvailableAccessPoints)
                .Include(u=>u.Owner)
               .AsNoTracking()
               .ToListAsync(cancellationToken);
        }

        public override async Task<KeyCard> GetOneAsync(Guid id, CancellationToken cancellationToken)
        {
            var entry =await base.GetOneAsync(id, cancellationToken);
            if (entry == null)
                return null;

            await Context.Entry(entry)
                .Collection(e => e.AvailableAccessPoints)
                .LoadAsync(cancellationToken);

            await Context.Entry(entry)
                .Reference(e => e.Owner)
                .LoadAsync(cancellationToken);

            return entry;
        }
    }
}
