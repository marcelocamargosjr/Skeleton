using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using sib_api_v3_sdk.Model;
using Skeleton.Domain.Core.Bus;
using Task = System.Threading.Tasks.Task;

namespace Skeleton.Functions.ServiceBusQueue.Consumers
{
    public class EmailQueueConsumer
    {
        private readonly IMessageHandler _message;

        public EmailQueueConsumer(IMessageHandler message)
        {
            _message = message;
        }

        [FunctionName("EmailQueueConsumer")]
        public async Task Run(
            [ServiceBusTrigger("%EmailQueueName%", Connection = "AzureWebJobsServiceBus")]
            SendSmtpEmail message,
            int deliveryCount,
            DateTime enqueuedTimeUtc,
            string messageId,
            ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {message.ToJson()}");
            log.LogInformation($"EnqueuedTimeUtc={enqueuedTimeUtc}");
            log.LogInformation($"DeliveryCount={deliveryCount}");
            log.LogInformation($"MessageId={messageId}");

            await _message.SendEmail(message);
        }
    }
}