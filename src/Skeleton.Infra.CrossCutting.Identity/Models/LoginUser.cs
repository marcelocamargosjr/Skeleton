using System.ComponentModel.DataAnnotations;

namespace Skeleton.Infra.CrossCutting.Identity.Models
{
    public class LoginUser
    {
        [Required(ErrorMessage = "O campo e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O campo e-mail está inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo senha é obrigatório.")]
        [StringLength(100, ErrorMessage = "O campo senha deve ter entre {2} e {1} caracteres.", MinimumLength = 6)]
        public string Password { get; set; }
    }
}