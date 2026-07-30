namespace GestionEstudiantesApi.DTOs;

public class EstudianteDto
{
    public int Id { get; set; }

    public string Matricula { get; set; } = string.Empty;

    public string Nombres { get; set; } = string.Empty;

    public string Apellidos { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public string Carrera { get; set; } = string.Empty;

    public DateTime FechaNacimiento { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaRegistro { get; set; }
}