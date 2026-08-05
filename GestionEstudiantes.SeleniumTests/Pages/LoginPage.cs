using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace GestionEstudiantes.SeleniumTests.Pages;

public sealed class LoginPage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _espera;

    private readonly By _formulario =
        By.Id("login-form");

    private readonly By _campoUsuario =
        By.Id("usuario");

    private readonly By _campoContrasena =
        By.Id("contrasena");

    private readonly By _botonIniciarSesion =
        By.Id("btn-iniciar-sesion");

    private readonly By _mensajeLogin =
        By.Id("mensaje-login");

    private readonly By _usuarioAutenticado =
        By.Id("usuario-autenticado");

    public LoginPage(
        IWebDriver driver,
        WebDriverWait espera)
    {
        _driver = driver;
        _espera = espera;
    }

    public void Abrir(string urlBase)
    {
        _driver.Navigate().GoToUrl(urlBase);
        EsperarPaginaCargada();
    }

    public void EsperarPaginaCargada()
    {
        _espera.Until(driver =>
        {
            try
            {
                return driver
                    .FindElement(_formulario)
                    .Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        });
    }

    public void EscribirCredenciales(
        string usuario,
        string contrasena)
    {
        var campoUsuario =
            _driver.FindElement(_campoUsuario);

        var campoContrasena =
            _driver.FindElement(_campoContrasena);

        campoUsuario.Clear();
        campoUsuario.SendKeys(usuario);

        campoContrasena.Clear();
        campoContrasena.SendKeys(contrasena);
    }

    public void EnviarFormulario()
    {
        _driver
            .FindElement(_botonIniciarSesion)
            .Click();
    }

    public void IniciarSesion(
        string usuario,
        string contrasena)
    {
        EscribirCredenciales(
            usuario,
            contrasena
        );

        EnviarFormulario();
    }

    public void EsperarIngresoExitoso()
    {
        _espera.Until(driver =>
            driver.Url.Contains(
                "estudiantes.html",
                StringComparison.OrdinalIgnoreCase
            )
        );

        _espera.Until(driver =>
        {
            try
            {
                return driver
                    .FindElement(_usuarioAutenticado)
                    .Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        });
    }

    public string EsperarMensajeDeError()
    {
        return _espera.Until(driver =>
        {
            try
            {
                var elemento =
                    driver.FindElement(_mensajeLogin);

                var texto =
                    elemento.Text.Trim();

                return elemento.Displayed &&
                       !string.IsNullOrWhiteSpace(texto)
                    ? texto
                    : null;
            }
            catch (NoSuchElementException)
            {
                return null;
            }
            catch (StaleElementReferenceException)
            {
                return null;
            }
        })!;
    }

    public string ObtenerUsuarioAutenticado()
    {
        return _driver
            .FindElement(_usuarioAutenticado)
            .Text
            .Trim();
    }

    public int ObtenerMaximoUsuario()
    {
        return ObtenerMaximoPermitido(
            _campoUsuario
        );
    }

    public int ObtenerMaximoContrasena()
    {
        return ObtenerMaximoPermitido(
            _campoContrasena
        );
    }

    public int ObtenerLongitudUsuario()
    {
        return ObtenerValorCampo(
            _campoUsuario
        ).Length;
    }

    public int ObtenerLongitudContrasena()
    {
        return ObtenerValorCampo(
            _campoContrasena
        ).Length;
    }

    private int ObtenerMaximoPermitido(
        By localizador)
    {
        var valor = _driver
            .FindElement(localizador)
            .GetAttribute("maxlength");

        if (
            string.IsNullOrWhiteSpace(valor) ||
            !int.TryParse(valor, out var maximo)
        )
        {
            throw new InvalidOperationException(
                $"El campo {localizador} no tiene un maxlength válido."
            );
        }

        return maximo;
    }

    private string ObtenerValorCampo(
        By localizador)
    {
        return _driver
            .FindElement(localizador)
            .GetAttribute("value")
            ?? string.Empty;
    }
}
