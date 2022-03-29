using System.ComponentModel.DataAnnotations;

namespace Skeleton.Application.ViewModels.v1.Customer
{
    public class RegisterNewCustomerViewModel
    {
        [Required(ErrorMessage = "O campo nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O campo nome deve ter entre {2} e {1} caracteres.", MinimumLength = 2)]
        public string Name { get; set; }

        [Required(ErrorMessage = "O campo e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O campo e-mail está inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo data de nascimento é obrigatório.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date, ErrorMessage = "O campo data de nascimento está inválido.")]
        public DateTime BirthDate { get; set; }
    }
}