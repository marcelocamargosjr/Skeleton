using FluentValidation.Results;
using MediatR;
using NetDevPack.Messaging;

namespace Skeleton.Domain.Core.Commands
{
    public abstract class Command : Message, IBaseRequest
    {
        public DateTime Timestamp { get; }
        public ValidationResult ValidationResult { get; set; }

        protected Command()
        {
            Timestamp = DateTime.Now;
            ValidationResult = new ValidationResult();
        }

        public virtual bool IsValid() => ValidationResult.IsValid;
    }
}