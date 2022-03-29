using NetDevPack.Messaging;

namespace Skeleton.Domain.Events.Customer
{
    public class CustomerUpdatedEvent : Event
    {
        public CustomerUpdatedEvent(Guid id, string name, string email, DateTime birthDate)
        {
            Id = id;
            Name = name;
            Email = email;
            BirthDate = birthDate;
            AggregateId = id;
        }

        public Guid Id { get; }
        public string Name { get; }
        public string Email { get; }
        public DateTime BirthDate { get; }
    }
}