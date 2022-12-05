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
    public class CamerasRepository : GenericRepositoryAsync<Camera>
    {
        public CamerasRepository(IDbContextFactory<AccessControlDbContext> context) : base(context)
        {
        }

        public override async Task<IEnumerable<Camera>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbSet.Include(entry => entry.AccessPoint)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public override async Task<Camera> GetOneAsync(Guid id, CancellationToken cancellationToken)
        {
            var entry = _dbSet.Find(new object[] { id });
            if (entry == null)
                return null;

            await Context.Entry(entry)
                 .Reference(e => e.AccessPoint)
                 .LoadAsync(cancellationToken);

            return entry;
        }

        public override void Delete(Camera entry)
        {
            base.Delete(entry);

            //no longer relevant events
            foreach (var fre in Context.FaceRecognizedEvents.Where(e => e.Camera.Id == entry.Id))
            {
                Context.FaceRecognizedEvents.Remove(fre);
            }
        }
    }
}
