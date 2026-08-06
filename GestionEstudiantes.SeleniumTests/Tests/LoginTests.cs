using GestionEstudiantes.SeleniumTests.Base;
using GestionEstudiantes.SeleniumTests.Pages;
using GestionEstudiantes.SeleniumTests.Utilities;

namespace GestionEstudiantes.SeleniumTests.Tests;

[TestFixture]
[NonParallelizable]
public class LoginTests : SeleniumTestBase
{
    private LoginPage _paginaLogin = null!;

    [SetUp]
    public void AbrirPaginaDeLogin()
    {
        _paginaLogin = new LoginPage(
            Driver,
            Espera
        );

        _paginaLogin.Abrir(UrlBase);
    }

    [Test]
    public void InicioSesion_CredencialesValidas_DebeIngresarAlSistema()
    {
        _paginaLogin.IniciarSesion(
            TestSettings.UsuarioValido,
            TestSettings.ContrasenaValida
        );

        _paginaLogin.EsperarIngresoExitoso();

        var usuarioMostrado =
            _paginaLogin.ObtenerUsuarioAutenticado();

        Assert.Multiple(() =>
        {
            Assert.That(
                Driver.Url,
                Does.Contain("estudiantes.html"),
                "El sistema no redirigió a la página de estudiantes."
            );

            Assert.That(
                usuarioMostrado,
                Does.Contain(
                    TestSettings.UsuarioValido
                ).IgnoreCase,
                "No se mostró el usuario autenticado."
            );
        });
    }

    [Test]
    public void InicioSesion_CredencialesIncorrectas_DebeMostrarError()
    {
        _paginaLogin.IniciarSesion(
            TestSettings.UsuarioValido,
            "Contrasena-Incorrecta-123"
        );

        var mensaje =
            _paginaLogin.EsperarMensajeDeError();

        Assert.Multiple(() =>
        {
            Assert.That(
                mensaje,
                Is.Not.Empty,
                "El sistema no mostró un mensaje de error."
            );

            Assert.That(
                Driver.Url,
                Does.Not.Contain("estudiantes.html"),
                "El sistema permitió entrar con una contraseña incorrecta."
            );
        });
    }

    [Test]
    public void InicioSesion_EntradasSobreElLimite_DebeRespetarMaxlength()
    {
        var maximoUsuario =
            _paginaLogin.ObtenerMaximoUsuario();

        var maximoContrasena =
            _paginaLogin.ObtenerMaximoContrasena();

        var usuarioExcesivo =
            new string(
                'u',
                maximoUsuario + 20
            );

        var contrasenaExcesiva =
            new string(
                'p',
                maximoContrasena + 20
            );

        _paginaLogin.EscribirCredenciales(
            usuarioExcesivo,
            contrasenaExcesiva
        );

        Assert.Multiple(() =>
        {
            Assert.That(
                _paginaLogin.ObtenerLongitudUsuario(),
                Is.EqualTo(maximoUsuario),
                "El campo usuario permitió superar su maxlength."
            );

            Assert.That(
                _paginaLogin.ObtenerLongitudContrasena(),
                Is.EqualTo(maximoContrasena),
                "El campo contraseña permitió superar su maxlength."
            );
        });

        _paginaLogin.EnviarFormulario();

        var mensaje =
            _paginaLogin.EsperarMensajeDeError();

        Assert.Multiple(() =>
        {
            Assert.That(
                mensaje,
                Is.Not.Empty,
                "No se mostró el rechazo de las credenciales."
            );

            Assert.That(
                Driver.Url,
                Does.Not.Contain("estudiantes.html"),
                "El sistema permitió entrar con datos fuera del límite."
            );
        });
    }
}
