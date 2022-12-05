using ACS.Core.Contracts.Services;
using ACS.Core.Data;
using ACS.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ACS.Core.Services.Repositories
{
    public class UserRepository : GenericRepositoryAsync<User>
    {
        public UserRepository(IDbContextFactory<AccessControlDbContext> context) : base(context)
        {
        }

        //public override void Create(User user)
        //{
        //    //mb recog service template here or smth
        //    _dbSet.Add(user);
        //}

        public override void Delete(User user)
        {
            base.Delete(user);
            foreach (var identifier in user.Identifiers)
            {
                Context.Identifiers.Remove(identifier);
            }
            foreach (var car in user.OwnedCars)
            {
                Context.Cars.Remove(car);
            }

            //no longer relevant events
            foreach (var ae in Context.AccessEvents.Where(e=>e.User.Id==user.Id))
            {
                Context.AccessEvents.Remove(ae);
            }
            foreach (var fre in Context.FaceRecognizedEvents.Where(e => e.RecognizedUser.Id == user.Id))
            {
                Context.FaceRecognizedEvents.Remove(fre);
            }
        }

        //public override void Delete(Guid id)
        //{
        //    Delete(GetOne(id));
        //}

        public override async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbSet.Include(u => u.Identifiers)
               .Include(u => u.KeyCards)
               .Include(u => u.OwnedCars)
               .AsNoTracking()
               .ToListAsync(cancellationToken);
        }

        public override async Task<User> GetOneAsync(Guid id, CancellationToken cancellationToken)
        {
            var user = await base.GetOneAsync(id, cancellationToken);
            if (user == null)
                return null;

            await Context.Entry(user)
                .Collection(p => p.Identifiers)
                .LoadAsync(cancellationToken);

            await Context.Entry(user)
               .Collection(p => p.KeyCards)
               .LoadAsync(cancellationToken);

            await Context.Entry(user)
               .Collection(p => p.OwnedCars)
               .LoadAsync(cancellationToken);

            return user;
        }
    }
}
