using ACS.Core.Data;
using ACS.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ACS.Core.Services.Repositories
{
    public class AccessPointRepository : GenericRepositoryAsync<AccessPoint>
    {
        public AccessPointRepository(AccessControlDbContext context) : base(context)
        {
        }

        public override void Delete(AccessPoint entry)
        {
            base.Delete(entry);
            //no longer relevant events
            foreach (var ae in Context.AccessEvents.Where(e=>e.AccessPoint.Id==entry.Id))
            {
                Context.AccessEvents.Remove(ae);
            }
        }

        public override async Task<IEnumerable<AccessPoint>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbSet.Include(u => u.Cameras)
               .Include(u => u.AllowedKeyCards)
               .AsNoTracking()
               .ToListAsync(cancellationToken);
        }

        public override async Task<AccessPoint> GetOneAsync(Guid id, CancellationToken cancellationToken)
        {
            var entry =await base.GetOneAsync(id, cancellationToken);
            if (entry == null)
                return null;

            await Context.Entry(entry)
                .Collection(e => e.Cameras)
                .LoadAsync(cancellationToken);

            await Context.Entry(entry)
               .Collection(e => e.AllowedKeyCards)
               .LoadAsync(cancellationToken);

            return entry;
        }
    }
}
