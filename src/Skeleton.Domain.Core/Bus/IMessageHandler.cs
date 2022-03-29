using sib_api_v3_sdk.Model;
using Task = System.Threading.Tasks.Task;

namespace Skeleton.Domain.Core.Bus
{
    public interface IMessageHandler
    {
        Task SendToQueue<T>(string connection, string queueName, T messageBody) where T : class;
        Task SendToQueue<T>(string queueName, T messageBody) where T : class;
        Task SendEmail(SendSmtpEmail sendSmtpEmail);
        Task SendTextMessage(string body, string from, string to);
    }
}