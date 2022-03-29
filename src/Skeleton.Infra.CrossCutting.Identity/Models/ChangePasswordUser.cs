using System.ComponentModel.DataAnnotations;

namespace Skeleton.Infra.CrossCutting.Identity.Models
{
    public class ChangePasswordUser
    {
        [Required(ErrorMessage = "O campo senha atual é obrigatório.")]
        [StringLength(100, ErrorMessage = "O campo senha atual deve ter entre {2} e {1} caracteres.", MinimumLength = 6)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "O campo nova senha é obrigatório.")]
        [StringLength(100, ErrorMessage = "O campo nova senha deve ter entre {2} e {1} caracteres.", MinimumLength = 6)]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "As senhas devem corresponder.")]
        public string PasswordConfirm { get; set; }
    }
}