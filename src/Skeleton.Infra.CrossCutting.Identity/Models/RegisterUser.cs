using System.ComponentModel.DataAnnotations;

namespace Skeleton.Infra.CrossCutting.Identity.Models
{
    public class RegisterUser
    {
        [Required(ErrorMessage = "O campo nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O campo nome deve ter entre {2} e {1} caracteres.", MinimumLength = 2)]
        public string Name { get; set; }

        [Required(ErrorMessage = "O campo e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O campo e-mail está inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo telefone é obrigatório.")]
        [Phone(ErrorMessage = "O campo telefone está inválido.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "O campo senha é obrigatório.")]
        [StringLength(100, ErrorMessage = "O campo senha deve ter entre {2} e {1} caracteres.", MinimumLength = 6)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "As senhas devem corresponder.")]
        public string PasswordConfirm { get; set; }
    }
}