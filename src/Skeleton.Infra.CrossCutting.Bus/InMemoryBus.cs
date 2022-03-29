using MediatR;
using NetDevPack.Messaging;
using Skeleton.Domain.Core.Bus;
using Skeleton.Domain.Core.Events;
using Command = Skeleton.Domain.Core.Commands.Command;

namespace Skeleton.Infra.CrossCutting.Bus
{
    public sealed class InMemoryBus : IMediatorHandler
    {
        private readonly IEventStore _eventStore;
        private readonly IMediator _mediator;

        public InMemoryBus(
            IEventStore eventStore,
            IMediator mediator)
        {
            _eventStore = eventStore;
            _mediator = mediator;
        }

        public async Task PublishEvent<T>(T @event) where T : Event
        {
            if (!@event.MessageType.Equals("DomainNotification"))
                _eventStore?.Save(@event);

            await _mediator.Publish(@event);
        }

        public async Task<object?> SendCommand<T>(T command) where T : Command
        {
            return await _mediator.Send(command);
        }
    }
}