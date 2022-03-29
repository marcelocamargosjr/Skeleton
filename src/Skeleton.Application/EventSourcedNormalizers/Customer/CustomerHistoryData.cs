namespace Skeleton.Application.EventSourcedNormalizers.Customer
{
    public class CustomerHistoryData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string BirthDate { get; set; }
        public string Action { get; set; }
        public string Timestamp { get; set; }
        public string Who { get; set; }
    }
}