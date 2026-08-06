using System.ComponentModel.DataAnnotations;

namespace GestionEstudiantesApi.DTOs;

public class LoginDto
{
    [Required(ErrorMessage = "El usuario es obligatorio.")]
    [StringLength(
        50,
        MinimumLength = 3,
        ErrorMessage = "El usuario debe tener entre 3 y 50 caracteres."
    )]
    public string Usuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(
        100,
        MinimumLength = 6,
        ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres."
    )]
    public string Contrasena { get; set; } = string.Empty;
}
