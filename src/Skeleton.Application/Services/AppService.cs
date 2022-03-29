using FluentValidation.Results;

namespace Skeleton.Application.Services
{
    public abstract class AppService
    {
        protected readonly ValidationResult ValidationResult;

        protected AppService()
        {
            ValidationResult = new ValidationResult();
        }

        protected void AddError(string mensagem) => ValidationResult.Errors.Add(new ValidationFailure(string.Empty, mensagem));
    }
}