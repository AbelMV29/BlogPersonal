using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlogPersonal.Models.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido"), EmailAddress(ErrorMessage = "Formato invalido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public DateTime? BirthDate { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Password { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido"),Compare(nameof(Password),ErrorMessage ="Las contraseñas deben coincidir")]
        public string ConfirmPassword { get; set; }
    }
}   
