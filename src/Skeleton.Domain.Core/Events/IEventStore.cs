using NetDevPack.Messaging;

namespace Skeleton.Domain.Core.Events
{
    public interface IEventStore
    {
        void Save<T>(T theEvent) where T : Event;
    }
}