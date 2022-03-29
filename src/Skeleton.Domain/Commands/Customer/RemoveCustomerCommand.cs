using FluentValidation.Results;
using MediatR;
using Skeleton.Domain.Commands.Customer.Validations;

namespace Skeleton.Domain.Commands.Customer
{
    public class RemoveCustomerCommand : CustomerCommand, IRequest<ValidationResult>
    {
        public RemoveCustomerCommand(Guid id)
        {
            Id = id;
            AggregateId = id;
        }

        public override bool IsValid()
        {
            ValidationResult = new RemoveCustomerCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}