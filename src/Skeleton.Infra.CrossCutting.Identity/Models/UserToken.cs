using NetDevPack.Identity.Jwt.Model;

namespace Skeleton.Infra.CrossCutting.Identity.Models
{
    public class UserToken
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public IEnumerable<UserClaim> Claims { get; set; }
    }
}