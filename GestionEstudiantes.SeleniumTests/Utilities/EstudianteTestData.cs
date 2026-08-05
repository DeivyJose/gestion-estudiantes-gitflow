namespace GestionEstudiantes.SeleniumTests.Utilities;

public sealed record EstudianteTestData(
    string Matricula,
    string Nombres,
    string Apellidos,
    string Correo,
    string Carrera,
    string FechaNacimiento,
    bool Activo = true
);
