using FluentValidation.Results;
using Skeleton.Application.EventSourcedNormalizers.Customer;
using Skeleton.Application.ViewModels.v1.Customer;

namespace Skeleton.Application.Interfaces
{
    public interface ICustomerAppService : IDisposable
    {
        Task<IEnumerable<CustomerViewModel>> GetAll();
        Task<object> GetById(Guid id);
        Task<object?> Register(RegisterNewCustomerViewModel customerViewModel);
        Task<ValidationResult?> Update(UpdateCustomerViewModel customerViewModel);
        Task<ValidationResult?> Remove(Guid id);
        Task<IList<CustomerHistoryData>> GetAllHistory(Guid id);
    }
}