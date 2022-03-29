using System.ComponentModel.DataAnnotations;

namespace Skeleton.Infra.CrossCutting.Identity.Models
{
    public class LoginTwoStepUser
    {
        [Required(ErrorMessage = "O campo código de dois fatores é obrigatório.")]
        public string TwoFactorCode { get; set; }
    }
}