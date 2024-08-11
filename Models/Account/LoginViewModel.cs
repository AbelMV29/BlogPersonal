using System.ComponentModel.DataAnnotations;

namespace BlogPersonal.Models.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="El campo {0} es requerido"),EmailAddress(ErrorMessage ="Formato invalido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Password { get; set; }
    }
}
