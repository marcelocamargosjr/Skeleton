using FluentValidation.Results;
using MediatR;
using Skeleton.Domain.Commands.Customer.Validations;

namespace Skeleton.Domain.Commands.Customer
{
    public class UpdateCustomerCommand : CustomerCommand, IRequest<ValidationResult>
    {
        public UpdateCustomerCommand(Guid id, string name, string email, DateTime birthDate)
        {
            Id = id;
            Name = name;
            Email = email;
            BirthDate = birthDate;
        }

        public override bool IsValid()
        {
            ValidationResult = new UpdateCustomerCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}