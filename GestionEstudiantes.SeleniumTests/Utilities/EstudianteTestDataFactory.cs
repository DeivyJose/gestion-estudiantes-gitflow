namespace GestionEstudiantes.SeleniumTests.Utilities;

public static class EstudianteTestDataFactory
{
    private static int _contador =
        Random.Shared.Next(1_000_000, 8_999_999);

    public static EstudianteTestData CrearValido()
    {
        var matricula = GenerarMatricula();

        return new EstudianteTestData(
            Matricula: matricula,
            Nombres: "Carlos",
            Apellidos: "Martinez",
            Correo: GenerarCorreo(matricula),
            Carrera: "Desarrollo de Software",
            FechaNacimiento: "2001-05-18",
            Activo: true
        );
    }

    public static EstudianteTestData CrearConValoresMinimos()
    {
        var matricula = GenerarMatricula();

        return new EstudianteTestData(
            Matricula: matricula,
            Nombres: "Al",
            Apellidos: "Li",
            Correo: GenerarCorreo(matricula),
            Carrera: "TI",
            FechaNacimiento: "2000-01-01",
            Activo: true
        );
    }

    public static EstudianteTestData CrearConMatriculaDuplicada(
        EstudianteTestData original)
    {
        return original with
        {
            Nombres = "Laura",
            Apellidos = "Duplicada",
            Correo = GenerarCorreo(
                original.Matricula
            )
        };
    }

    private static string GenerarMatricula()
    {
        var numero =
            Interlocked.Increment(ref _contador)
            % 10_000_000;

        return $"8{numero:0000000}";
    }

    private static string GenerarCorreo(
        string matricula)
    {
        var identificador =
            Guid.NewGuid()
                .ToString("N")[..8];

        return
            $"qa.{matricula}.{identificador}@example.com";
    }
}
