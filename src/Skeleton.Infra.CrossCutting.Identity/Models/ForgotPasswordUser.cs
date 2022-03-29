using System.ComponentModel.DataAnnotations;

namespace Skeleton.Infra.CrossCutting.Identity.Models
{
    public class ForgotPasswordUser
    {
        [Required(ErrorMessage = "O campo e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O campo e-mail está inválido.")]
        public string Email { get; set; }
    }
}