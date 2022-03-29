using FluentValidation;

namespace Skeleton.Domain.Commands.Customer.Validations
{
    public abstract class CustomerValidation<T> : AbstractValidator<T> where T : CustomerCommand
    {
        protected void ValidateId()
        {
            RuleFor(c => c.Id)
                .NotEqual(Guid.Empty).WithMessage("O campo identificação é obrigatório.");
        }

        protected void ValidateName()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("O campo nome é obrigatório.")
                .Length(2, 100).WithMessage("O campo nome deve ter entre 2 e 100 caracteres.");
        }

        protected void ValidateEmail()
        {
            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("O campo e-mail é obrigatório.")
                .EmailAddress().WithMessage("O campo e-mail está inválido.");
        }

        protected void ValidateBirthDate()
        {
            RuleFor(c => c.BirthDate)
                .Must(HaveMinimumAge).WithMessage("O cliente deve ter 18 anos ou mais.");
        }

        private static bool HaveMinimumAge(DateTime birthDate)
        {
            return birthDate <= DateTime.Now.AddYears(-18);
        }
    }
}