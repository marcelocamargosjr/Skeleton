namespace Skeleton.Infra.CrossCutting.Identity.Models
{
    public class SignInResponse
    {
        public string StatusMessage { get; set; }
        public AuthResponse? AuthResponse { get; set; }
    }
}