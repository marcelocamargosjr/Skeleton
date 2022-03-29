namespace Skeleton.Domain.Commands.Customer.Validations
{
    public class RegisterNewCustomerCommandValidation : CustomerValidation<RegisterNewCustomerCommand>
    {
        public RegisterNewCustomerCommandValidation()
        {
            ValidateName();
            ValidateEmail();
            ValidateBirthDate();
        }
    }
}