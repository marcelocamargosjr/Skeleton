namespace Skeleton.Services.Api.Models
{
    public class ProblemDetails
    {
        public string? Title { get; set; }
        public int? Status { get; set; }
        public string? Instance { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
    }
}