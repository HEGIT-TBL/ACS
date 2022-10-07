using ACS.Core.Contracts.Services;
using ACS.Core.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;

namespace ACS.Core.Services.Repositories
{
    public class GenericRepositoryAsync<TEntity> : IGenericRepositoryAsync<TEntity> where TEntity : class, IHasGuid
    {
        public AccessControlDbContext Context { get; }
        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepositoryAsync(AccessControlDbContext context)
        {
            Context = context;
            _dbSet = Context.Set<TEntity>();
        }

        public virtual EntityEntry<TEntity> Attach(TEntity item)
        {
            return _dbSet.Attach(item);
        }

        public virtual async Task CreateAsync(TEntity item, CancellationToken cancellationToken)
        {
            await _dbSet.AddAsync(item, cancellationToken);
        }

        public virtual void Delete(TEntity item)
        {
            if (item != null)
                _dbSet.Remove(item);
        }

        public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            Delete(await GetOneAsync(id, cancellationToken));
        }

        public async Task<bool> ElementExistsAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbSet.AsNoTracking().AnyAsync(item => item.Id == id, cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            var set = await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
            return set;
        }

        public virtual async Task<TEntity> GetOneAsync(Guid id, CancellationToken cancellationToken)
        {
            var item = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
            return item ?? null;
        }

        public virtual void Update(TEntity otherItem)
        {
            _dbSet.Update(otherItem);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await Context.SaveChangesAsync(cancellationToken);
        }

        //TODO: test this bs consider it done for now
        //private void ExecuteForEachIncludedEntityCollection(IEnumerable<object> entities, string nameOfMethod)
        //{
        //    void ExecuteForEachIncludedEntity(object entity, string nameOfMethod)
        //    {
        //        var entityType = entity.GetType();
        //        var entitySetType = typeof(DbSet<>).MakeGenericType(entityType);

        //        var dbSet = Context
        //            .GetType()
        //            .GetProperties()
        //            .FirstOrDefault(prop => prop.Name.StartsWith(entityType.Name))
        //            .GetValue(Context);
        //        var method = dbSet.GetType().GetMethod(nameOfMethod);
        //        method.Invoke(dbSet, new object[] { entity });

        //        var properties = entityType.GetProperties();

        //        foreach (var propertyInfo in properties)
        //        {
        //            var propertyType = propertyInfo.PropertyType;
        //            var isCollection = propertyType
        //               .GetInterfaces()
        //               .Any(x => x == typeof(IEnumerable)) && !propertyType.Equals(typeof(string));

        //            if (!isCollection && !propertyType.IsClass || propertyType.Equals(typeof(string)) || propertyType.Equals(typeof(byte[])) || propertyType.Equals(typeof(float[])))
        //            {
        //                continue;
        //            }
        //            else
        //            {
        //                //load collections from nav props
        //                var entry = Context.Entry(entity);
        //                if (isCollection)
        //                {
        //                    if (entry.Member(propertyInfo.Name) as CollectionEntry == null)
        //                        continue;

        //                    entry.Collection(propertyInfo.Name).Load();
        //                }
        //                else
        //                {
        //                    if (entry.Member(propertyInfo.Name) as ReferenceEntry == null)
        //                        continue;

        //                    entry.Reference(propertyInfo.Name).Load();
        //                }
        //                var propertyValue = propertyInfo.GetValue(entity);
        //                if (propertyValue == null)
        //                    continue;

        //                if (isCollection)
        //                {
        //                    ExecuteForEachIncludedEntityCollection((IEnumerable<object>)propertyValue, nameOfMethod);
        //                }
        //                else
        //                {
        //                    ExecuteForEachIncludedEntity(propertyValue, nameOfMethod);
        //                }
        //            }
        //        }
        //    }
        //    foreach (var entity in entities)
        //        ExecuteForEachIncludedEntity(entity, nameOfMethod);

        //}
        ////private static bool GetPropInfoCondition(PropertyInfo propInfo, bool onlyCollection)
        ////{
        ////    var propertyType = propInfo.PropertyType;
        ////    var isCollection = propertyType
        ////       .GetInterfaces()
        ////       .Any(x => x == typeof(IEnumerable)) && !propertyType.Equals(typeof(string));
        ////    return onlyCollection
        ////        ? isCollection
        ////        : !isCollection && (propertyType.IsValueType || propertyType.Equals(typeof(string)));
        ////}

        //private void IncludeAllList(IEnumerable entities, List<object> entitiesLoaded = null)
        //{
        //    if (entitiesLoaded == null)
        //        entitiesLoaded = new List<object>();

        //    void IncludeAllEntity(object entity, List<object> entitiesLoaded)
        //    {
        //        if (entitiesLoaded.Contains(entity))
        //            return;

        //        entitiesLoaded.Add(entity);

        //        var properties = entity.GetType().GetProperties();

        //        foreach (var propertyInfo in properties)
        //        {
        //            var propertyType = propertyInfo.PropertyType;
        //            var isCollection = propertyType
        //               .GetInterfaces()
        //               .Any(x => x == typeof(IEnumerable)) && !propertyType.Equals(typeof(string));

        //            if (!isCollection && (propertyType.IsValueType || propertyType.Equals(typeof(string))))
        //            {
        //                continue;
        //            }
        //            else
        //            {
        //                var entry = Context.Entry(entity);
        //                if (isCollection)
        //                {
        //                    if (entry.Member(propertyInfo.Name) as CollectionEntry == null)
        //                        continue;

        //                    entry.Collection(propertyInfo.Name).Load();
        //                }
        //                else
        //                {
        //                    if (entry.Member(propertyInfo.Name) as ReferenceEntry == null)
        //                        continue;

        //                    entry.Reference(propertyInfo.Name).Load();
        //                }
        //                var propertyValue = propertyInfo.GetValue(entity);

        //                if (propertyValue == null)
        //                    continue;
        //                if (isCollection)
        //                {
        //                    IncludeAllList((IEnumerable)propertyValue, entitiesLoaded);
        //                }
        //                else
        //                {
        //                    IncludeAllEntity(propertyValue, entitiesLoaded);
        //                }
        //            }
        //            #region
        //            ////right ver from stackovflw
        //            //if (isCollection)
        //            //{
        //            //    var entry = context.Entry(entity);

        //            //    if (entry.Member(propertyInfo.Name) as CollectionEntry == null)
        //            //        continue;

        //            //    entry.Collection(propertyInfo.Name).Load();

        //            //    var propertyValue = propertyInfo.GetValue(entity);

        //            //    if (propertyValue == null)
        //            //        continue;

        //            //    EnumerateAllIncludesList(context, (IEnumerable)propertyValue, entitiesLoaded);
        //            //}
        //            //else if (!propertyType.IsValueType && !propertyType.Equals(typeof(string)))
        //            //{
        //            //    var entry = context.Entry(entity);

        //            //    if (entry.Member(propertyInfo.Name) as ReferenceEntry == null)
        //            //        continue;

        //            //    entry.Reference(propertyInfo.Name).Load();

        //            //    var propertyValue = propertyInfo.GetValue(entity);

        //            //    if (propertyValue == null)
        //            //        continue;

        //            //    EnumerateAllIncludesEntity(context, propertyValue, entitiesLoaded);
        //            //}
        //            //else
        //            //{
        //            //    continue;
        //            //}
        //            #endregion
        //        }
        //    }

        //    foreach (var entity in entities)
        //        IncludeAllEntity(entity, entitiesLoaded);
        //}
    }
}
