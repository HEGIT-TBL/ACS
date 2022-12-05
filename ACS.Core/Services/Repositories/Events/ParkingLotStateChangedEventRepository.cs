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
    public class ParkingLotStateChangedEventRepository : GenericRepositoryAsync<ParkingLotStateChangedEvent>
    {
        public ParkingLotStateChangedEventRepository(IDbContextFactory<AccessControlDbContext> context) : base(context)
        {
        }

        public override async Task<IEnumerable<ParkingLotStateChangedEvent>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbSet.Include(entry => entry.ChangedLot)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public override async Task<ParkingLotStateChangedEvent> GetOneAsync(Guid id, CancellationToken cancellationToken)
        {
            var entry = _dbSet.Find(new object[] { id });
            if (entry == null)
                return null;

           await Context.Entry(entry)
                .Reference(e => e.ChangedLot)
                .LoadAsync(cancellationToken);

            return entry;
        }

        public override void Update(ParkingLotStateChangedEvent otherItem)
        {
            return;
        }

        public override void Delete(ParkingLotStateChangedEvent item)
        {
            return;
        }

        public override Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
