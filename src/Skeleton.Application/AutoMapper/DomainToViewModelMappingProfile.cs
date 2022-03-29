using AutoMapper;
using Skeleton.Application.ViewModels.v1.Customer;
using Skeleton.Domain.Models;

namespace Skeleton.Application.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            // Customer
            CreateMap<Customer, CustomerViewModel>();
        }
    }
}