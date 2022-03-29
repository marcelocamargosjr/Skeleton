using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Data;
using NetDevPack.Domain;
using Skeleton.Infra.Data.Context;

namespace Skeleton.Infra.Data.Repository
{
    public abstract class Repository<TEntity> : Domain.Interfaces.IRepository<TEntity> where TEntity : class, IAggregateRoot
    {
        protected readonly SkeletonContext Db;
        protected readonly DbSet<TEntity> DbSet;

        protected Repository(SkeletonContext context)
        {
            Db = context;
            DbSet = Db.Set<TEntity>();
        }

        public IUnitOfWork UnitOfWork => Db;

        public virtual async Task<IList<TEntity>> GetAll()
        {
            return await DbSet.ToListAsync();
        }

        public virtual async Task<TEntity?> GetById(Guid id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual async Task<IList<TEntity>> Find(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "",
            int? page = null,
            int? pageSize = null)
        {
            IQueryable<TEntity> query = DbSet;

            if (filter is not null)
            {
                query = query.Where(filter);
            }

            query = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            if (orderBy is not null)
            {
                query = orderBy(query);
            }

            if (page is not null && pageSize is not null)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            return await query.ToListAsync();
        }

        public virtual void Add(TEntity obj)
        {
            DbSet.Add(obj);
        }

        public virtual void Update(TEntity obj)
        {
            DbSet.Update(obj);
        }

        public virtual void Remove(TEntity obj)
        {
            DbSet.Remove(obj);
        }

        public void Dispose()
        {
            Db.Dispose();
        }
    }
}