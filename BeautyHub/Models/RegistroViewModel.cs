using System.ComponentModel.DataAnnotations;

namespace BeautyHub.Models
{
    public class RegistroViewModel
    {
        [Required(ErrorMessage = "Elige el tipo de cuenta.")]
        [Display(Name = "Tipo de cuenta")]
        public string Tipo { get; set; } = Roles.Cliente;

        [Required(ErrorMessage = "Ingresa tus nombres."), MaxLength(100)]
        public string Nombres { get; set; } = "";

        [Required(ErrorMessage = "Ingresa tus apellidos."), MaxLength(100)]
        public string Apellidos { get; set; } = "";

        [Required(ErrorMessage = "Ingresa un teléfono."), MaxLength(20)]
        [RegularExpression(@"^[0-9+\s()-]{7,20}$", ErrorMessage = "Teléfono no válido.")]
        public string Telefono { get; set; } = "";

        [EmailAddress(ErrorMessage = "Correo no válido."), MaxLength(100)]
        public string? Correo { get; set; }

        [MaxLength(100)]
        public string? Especialidad { get; set; }

        [Required(ErrorMessage = "Elige un nombre de usuario."), MinLength(4), MaxLength(50)]
        [RegularExpression(@"^[A-Za-z0-9_.]+$", ErrorMessage = "Solo letras, números, punto o guion bajo.")]
        public string Usuario { get; set; } = "";

        [Required(ErrorMessage = "Escribe una contraseña."), DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string Contrasena { get; set; } = "";

        [Required(ErrorMessage = "Confirma la contraseña."), DataType(DataType.Password)]
        [Compare(nameof(Contrasena), ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarContrasena { get; set; } = "";
    }
}
