using System.ComponentModel.DataAnnotations;

namespace BlogPersonal.Models.Account
{
    public class LoginViewModel
    {
        [Display(Name = "correo electrónico")]
        [Required(ErrorMessage = "El {0} es requerido")]
        [EmailAddress(ErrorMessage = "Formato inválido")]
        public string Email { get; set; }
        [Display(Name = "contraseña")]
        [Required(ErrorMessage = "La {0} es requerida")]
        public string Password { get; set; }
    }
}
