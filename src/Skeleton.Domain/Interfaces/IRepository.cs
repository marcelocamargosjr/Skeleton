using System.Linq.Expressions;
using NetDevPack.Domain;

namespace Skeleton.Domain.Interfaces
{
    public interface IRepository<TEntity> : NetDevPack.Data.IRepository<TEntity> where TEntity : IAggregateRoot
    {
        Task<IList<TEntity>> GetAll();
        Task<TEntity?> GetById(Guid id);

        Task<IList<TEntity>> Find(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "",
            int? page = null,
            int? pageSize = null);

        void Add(TEntity obj);
        void Update(TEntity obj);
        void Remove(TEntity obj);
    }
}