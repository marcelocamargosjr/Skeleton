using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Skeleton.Domain.Core.Bus;
using Skeleton.Domain.Core.Utilities;
using Skeleton.Functions.ServiceBusQueue.Models;

namespace Skeleton.Functions.ServiceBusQueue.Consumers
{
    public class SmsQueueConsumer
    {
        private readonly IMessageHandler _message;

        public SmsQueueConsumer(IMessageHandler message)
        {
            _message = message;
        }

        [FunctionName("SmsQueueConsumer")]
        public async Task Run(
            [ServiceBusTrigger("%SmsQueueName%", Connection = "AzureWebJobsServiceBus")]
            TextMessage message,
            int deliveryCount,
            DateTime enqueuedTimeUtc,
            string messageId,
            ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {message.ToJson()}");
            log.LogInformation($"EnqueuedTimeUtc={enqueuedTimeUtc}");
            log.LogInformation($"DeliveryCount={deliveryCount}");
            log.LogInformation($"MessageId={messageId}");

            await _message.SendTextMessage(message.Body, message.From, message.To);
        }
    }
}