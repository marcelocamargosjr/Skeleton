using Skeleton.Domain.Core.Bus;
using Skeleton.Domain.Core.Utilities;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using Skeleton.Infra.CrossCutting.Bus.Configurations;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Task = System.Threading.Tasks.Task;

namespace Skeleton.Infra.CrossCutting.Bus
{
    public sealed class MessageServiceBus : IMessageHandler
    {
        private readonly MessageServiceBusConfig _messageServiceBus;

        public MessageServiceBus(IOptions<MessageServiceBusConfig> messageServiceBus)
        {
            _messageServiceBus = messageServiceBus.Value;
        }

        public async Task SendToQueue<T>(string connection, string queueName, T messageBody) where T : class
        {
            if (string.IsNullOrWhiteSpace(connection)) throw new ArgumentNullException(nameof(connection));

            if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentNullException(nameof(queueName));

            switch (messageBody)
            {
                case null: throw new ArgumentNullException(nameof(messageBody));
                case string message when string.IsNullOrWhiteSpace(message): throw new ArgumentNullException(nameof(messageBody));
            }

            await using var client = new ServiceBusClient(connection);
            var sender = client.CreateSender(queueName);
            await sender.SendMessageAsync(new ServiceBusMessage(messageBody is string body ? body : messageBody.ToJson()));
        }

        public async Task SendToQueue<T>(string queueName, T messageBody) where T : class => await SendToQueue(_messageServiceBus.AzureServiceBusConnectionString, queueName, messageBody);

        public async Task SendEmail(SendSmtpEmail sendSmtpEmail)
        {
            Configuration.Default.AddApiKey("api-key", _messageServiceBus.SendinblueApiKey);
            var apiInstance = new TransactionalEmailsApi();
            var message = await apiInstance.SendTransacEmailAsync(sendSmtpEmail);

            if (string.IsNullOrWhiteSpace(message.MessageId) && !message.MessageIds.Any())
                throw new Exception(nameof(message));
        }

        public async Task SendTextMessage(string body, string from, string to)
        {
            TwilioClient.Init(_messageServiceBus.TwilioAccountSid, _messageServiceBus.TwilioAuthToken);
            var message = await MessageResource.CreateAsync(body: body, from: new PhoneNumber(from), to: new PhoneNumber(to));

            if (message.Status.Equals(MessageResource.StatusEnum.Failed))
                throw new Exception(nameof(message));
        }
    }
}