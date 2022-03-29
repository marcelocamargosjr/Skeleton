using MediatR;
using Skeleton.Domain.Commands.Customer.Validations;

namespace Skeleton.Domain.Commands.Customer
{
    public class RegisterNewCustomerCommand : CustomerCommand, IRequest<object>
    {
        public RegisterNewCustomerCommand(string name, string email, DateTime birthDate)
        {
            Name = name;
            Email = email;
            BirthDate = birthDate;
        }

        public override bool IsValid()
        {
            ValidationResult = new RegisterNewCustomerCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}