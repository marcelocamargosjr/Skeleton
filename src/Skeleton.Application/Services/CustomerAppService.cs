using AutoMapper;
using FluentValidation.Results;
using Skeleton.Application.EventSourcedNormalizers.Customer;
using Skeleton.Application.Interfaces;
using Skeleton.Application.ViewModels.v1.Customer;
using Skeleton.Domain.Commands.Customer;
using Skeleton.Domain.Core.Bus;
using Skeleton.Domain.Interfaces;
using Skeleton.Domain.Models;
using Skeleton.Infra.Data.Repository.EventSourcing;

namespace Skeleton.Application.Services
{
    public class CustomerAppService : AppService, ICustomerAppService
    {
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEventStoreRepository _eventStoreRepository;
        private readonly IMediatorHandler _mediator;

        public CustomerAppService(
            IMapper mapper,
            ICustomerRepository customerRepository,
            IEventStoreRepository eventStoreRepository,
            IMediatorHandler mediator)
        {
            _mapper = mapper;
            _customerRepository = customerRepository;
            _eventStoreRepository = eventStoreRepository;
            _mediator = mediator;
        }

        public async Task<IEnumerable<CustomerViewModel>> GetAll()
        {
            return _mapper.Map<IEnumerable<CustomerViewModel>>(await _customerRepository.GetAll());
        }

        public async Task<object> GetById(Guid id)
        {
            var customer = await _customerRepository.GetById(id);

            if (customer is null)
            {
                AddError("O registro não existe.");
                return ValidationResult;
            }

            return _mapper.Map<CustomerViewModel>(customer);
        }

        public async Task<object?> Register(RegisterNewCustomerViewModel customerViewModel)
        {
            var registerCommand = _mapper.Map<RegisterNewCustomerCommand>(customerViewModel);
            var registerResult = await _mediator.SendCommand(registerCommand);

            return registerResult is not Customer ? (ValidationResult?)registerResult : _mapper.Map<CustomerViewModel>(registerResult);
        }

        public async Task<ValidationResult?> Update(UpdateCustomerViewModel customerViewModel)
        {
            var updateCommand = _mapper.Map<UpdateCustomerCommand>(customerViewModel);

            return (ValidationResult?)await _mediator.SendCommand(updateCommand);
        }

        public async Task<ValidationResult?> Remove(Guid id)
        {
            var removeCommand = new RemoveCustomerCommand(id);

            return (ValidationResult?)await _mediator.SendCommand(removeCommand);
        }

        public async Task<IList<CustomerHistoryData>> GetAllHistory(Guid id)
        {
            return CustomerHistory.ToJavaScriptCustomerHistory(await _eventStoreRepository.All(id));
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}