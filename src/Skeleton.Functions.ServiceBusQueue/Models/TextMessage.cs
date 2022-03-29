namespace Skeleton.Functions.ServiceBusQueue.Models
{
    public class TextMessage
    {
        public string Body { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}