using Skeleton.Domain.Interfaces;
using Skeleton.Domain.Models;
using Skeleton.Infra.Data.Context;

namespace Skeleton.Infra.Data.Repository
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(SkeletonContext context) : base(context)
        {
        }
    }
}