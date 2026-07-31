using System.ComponentModel.DataAnnotations;

namespace GestionEstudiantesApi.DTOs;

public class CrearEstudianteDto : IValidatableObject
{
    [Required(ErrorMessage = "La matrícula es obligatoria.")]
    [RegularExpression(
        @"^\d{8}$",
        ErrorMessage = "La matrícula debe contener exactamente 8 números."
    )]
    public string Matricula { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "Los nombres deben tener entre 2 y 100 caracteres."
    )]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos son obligatorios.")]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "Los apellidos deben tener entre 2 y 100 caracteres."
    )]
    public string Apellidos { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [StringLength(
        150,
        ErrorMessage = "El correo no puede superar los 150 caracteres."
    )]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La carrera es obligatoria.")]
    [StringLength(
        150,
        MinimumLength = 2,
        ErrorMessage = "La carrera debe tener entre 2 y 150 caracteres."
    )]
    public string Carrera { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    public DateTime? FechaNacimiento { get; set; }

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (FechaNacimiento.HasValue &&
            FechaNacimiento.Value.Date > DateTime.UtcNow.Date)
        {
            yield return new ValidationResult(
                "La fecha de nacimiento no puede ser posterior a la fecha actual.",
                new[] { nameof(FechaNacimiento) }
            );
        }
    }
}