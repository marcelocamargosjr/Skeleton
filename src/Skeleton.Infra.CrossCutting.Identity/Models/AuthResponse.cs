namespace Skeleton.Infra.CrossCutting.Identity.Models
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public double ExpiresIn { get; set; }
        public string UniqueName { get; set; }
        public UserToken UserToken { get; set; }
    }
}