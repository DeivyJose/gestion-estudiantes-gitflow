namespace GestionEstudiantes.SeleniumTests.Utilities;

public static class TestSettings
{
    public static string UsuarioValido =>
        ObtenerVariableObligatoria(
            "TEST_LOGIN_USER"
        );

    public static string ContrasenaValida =>
        ObtenerVariableObligatoria(
            "TEST_LOGIN_PASSWORD"
        );

    private static string ObtenerVariableObligatoria(
        string nombre)
    {
        var valor =
            Environment.GetEnvironmentVariable(nombre);

        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new InvalidOperationException(
                $"Debes configurar la variable de entorno {nombre} " +
                "antes de ejecutar las pruebas."
            );
        }

        return valor;
    }
}
