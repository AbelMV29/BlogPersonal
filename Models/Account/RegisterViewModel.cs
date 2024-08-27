using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlogPersonal.Models.Account
{
    public class RegisterViewModel
    {
        [Display(Name = "nombre")]
        [Required(ErrorMessage = "El {0} es requerido")]
        public string FirstName { get; set; }

        [Display(Name = "apellido")]
        [Required(ErrorMessage = "El {0} es requerido")]
        public string LastName { get; set; }

        [Display(Name = "correo electrónico")]
        [Required(ErrorMessage = "El {0} es requerido")]
        [EmailAddress(ErrorMessage = "Formato inválido")]
        public string Email { get; set; }

        [Display(Name = "fecha de eacimiento")]
        [Required(ErrorMessage = "La {0} es requerida")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "contraseña")]
        [Required(ErrorMessage = "La {0} es requerida")]
        [Length(8, 15, ErrorMessage = "La {0} debe tener entre [8-15] caracteres")]
        public string Password { get; set; }

        [Display(Name = "confirmación de contraseña")]
        [Required(ErrorMessage ="La {0} es requerida")]
        [Compare(nameof(Password), ErrorMessage = "Las contraseñas deben coincidir")]
        public string ConfirmPassword { get; set; }
    }
}   
