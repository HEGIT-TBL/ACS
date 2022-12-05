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
    public class ParkingLotRepository : GenericRepositoryAsync<ParkingLot>
    {
        public ParkingLotRepository(IDbContextFactory<AccessControlDbContext> context) : base(context)
        {
        }

        public override async Task<IEnumerable<ParkingLot>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbSet.Include(entry => entry.PlacedCar)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public override async Task<ParkingLot> GetOneAsync(Guid id, CancellationToken cancellationToken)
        {
            var entry = _dbSet.Find(new object[] { id });
            if (entry == null)
                return null;

            await Context.Entry(entry)
                 .Reference(e => e.PlacedCar)
                 .LoadAsync(cancellationToken);

            return entry;
        }

        public override void Delete(ParkingLot entry)
        {
            base.Delete(entry);

            //no longer relevant events
            foreach (var plsce in Context.ParkingLotStateChangedEvents.Where(e => e.ChangedLot.Id == entry.Id))
            {
                Context.ParkingLotStateChangedEvents.Remove(plsce);
            }
        }
    }
}
