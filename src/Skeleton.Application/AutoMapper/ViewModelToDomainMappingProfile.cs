using AutoMapper;
using Skeleton.Application.ViewModels.v1.Customer;
using Skeleton.Domain.Commands.Customer;

namespace Skeleton.Application.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            // Customer
            CreateMap<RegisterNewCustomerViewModel, RegisterNewCustomerCommand>()
                .ConstructUsing(src => new RegisterNewCustomerCommand(src.Name, src.Email, src.BirthDate));

            CreateMap<UpdateCustomerViewModel, UpdateCustomerCommand>()
                .ConstructUsing(src => new UpdateCustomerCommand(src.Id, src.Name, src.Email, src.BirthDate));
        }
    }
}