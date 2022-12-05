using ACS.Core.Data;
using ACS.Core.Models.Events;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ACS.Core.Services.Repositories.Events
{
    public class FaceRecognizedEventRepository : GenericRepositoryAsync<FaceRecognizedEvent>
    {
        public FaceRecognizedEventRepository(IDbContextFactory<AccessControlDbContext> context) : base(context)
        {
        }

        public override async Task<IEnumerable<FaceRecognizedEvent>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbSet.Include(entry => entry.RecognizedUser)
                .Include(entry => entry.Camera)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public override async Task<FaceRecognizedEvent> GetOneAsync(Guid id, CancellationToken cancellationToken)
        {
            var entry = _dbSet.Find(new object[] { id });
            if (entry == null)
                return null;

            await Context.Entry(entry)
                 .Reference(e => e.Camera)
                 .LoadAsync(cancellationToken);

            await Context.Entry(entry)
                 .Reference(e => e.RecognizedUser)
                 .LoadAsync(cancellationToken);

            return entry;
        }

        public override void Update(FaceRecognizedEvent otherItem)
        {
            return;
        }

        public override void Delete(FaceRecognizedEvent item)
        {
            return;
        }

        public override Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
