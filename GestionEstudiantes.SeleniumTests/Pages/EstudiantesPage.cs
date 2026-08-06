using GestionEstudiantes.SeleniumTests.Utilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace GestionEstudiantes.SeleniumTests.Pages;

public sealed class EstudiantesPage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _espera;

    private readonly By _formulario =
        By.Id("formulario-estudiante");

    private readonly By _matricula =
        By.Id("matricula");

    private readonly By _nombres =
        By.Id("nombres");

    private readonly By _apellidos =
        By.Id("apellidos");

    private readonly By _correo =
        By.Id("correo");

    private readonly By _carrera =
        By.Id("carrera");

    private readonly By _fechaNacimiento =
        By.Id("fecha-nacimiento");

    private readonly By _activo =
        By.Id("activo");

    private readonly By _botonGuardar =
        By.Id("btn-guardar-estudiante");

    private readonly By _botonLimpiarFormulario =
        By.Id("btn-limpiar-formulario");

    private readonly By _campoBusqueda =
        By.Id("buscar-estudiante");

    private readonly By _filasTabla =
        By.CssSelector(
            "#cuerpo-tabla-estudiantes tr"
        );

    private readonly By _mensajeFormulario =
        By.Id("mensaje-formulario");

    private readonly By _mensajeGlobal =
        By.Id("mensaje-global");

    private readonly By _modalEliminar =
        By.Id("modal-eliminar");

    private readonly By _botonConfirmarEliminacion =
        By.Id("btn-confirmar-eliminacion");

    public EstudiantesPage(
        IWebDriver driver,
        WebDriverWait espera)
    {
        _driver = driver;
        _espera = espera;
    }

    public void EsperarPaginaCargada()
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
                    .FindElement(_formulario)
                    .Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        });
    }

    public void Registrar(
        EstudianteTestData estudiante)
    {
        CompletarFormulario(estudiante);

        _driver
            .FindElement(_botonGuardar)
            .Click();
    }

    public void CompletarFormulario(
        EstudianteTestData estudiante)
    {
        Escribir(
            _matricula,
            estudiante.Matricula
        );

        Escribir(
            _nombres,
            estudiante.Nombres
        );

        Escribir(
            _apellidos,
            estudiante.Apellidos
        );

        Escribir(
            _correo,
            estudiante.Correo
        );

        Escribir(
            _carrera,
            estudiante.Carrera
        );

        Escribir(
            _fechaNacimiento,
            estudiante.FechaNacimiento
        );

        var casillaActivo =
            _driver.FindElement(_activo);

        if (
            casillaActivo.Selected !=
            estudiante.Activo
        )
        {
            casillaActivo.Click();
        }
    }

    public IWebElement EsperarEstudianteEnTabla(
        string matricula)
    {
        Buscar(matricula);

        return _espera.Until(driver =>
        {
            try
            {
                return driver
                    .FindElements(_filasTabla)
                    .FirstOrDefault(fila =>
                        fila.Text.Contains(
                            matricula,
                            StringComparison.OrdinalIgnoreCase
                        )
                    );
            }
            catch (StaleElementReferenceException)
            {
                return null;
            }
        })!;
    }

    public void EsperarCantidadDeEstudiantes(
        string matricula,
        int cantidadEsperada)
    {
        _espera.Until(_ =>
        {
            try
            {
                return ContarEstudiantes(
                    matricula
                ) == cantidadEsperada;
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
        });
    }

    public int ContarEstudiantes(
        string matricula)
    {
        return _driver
            .FindElements(_filasTabla)
            .Count(fila =>
                fila.Text.Contains(
                    matricula,
                    StringComparison.OrdinalIgnoreCase
                )
            );
    }

    public void Buscar(string texto)
    {
        var campo =
            _driver.FindElement(
                _campoBusqueda
            );

        campo.Clear();
        campo.SendKeys(texto);
    }

    public void LimpiarFormulario()
    {
        _driver
            .FindElement(
                _botonLimpiarFormulario
            )
            .Click();

        _espera.Until(driver =>
        {
            var valor = driver
                .FindElement(_matricula)
                .GetAttribute("value");

            return string.IsNullOrEmpty(valor);
        });
    }

    public string ObtenerMensajeActual()
    {
        var mensajeFormulario =
            ObtenerTextoVisible(
                _mensajeFormulario
            );

        if (
            !string.IsNullOrWhiteSpace(
                mensajeFormulario
            )
        )
        {
            return mensajeFormulario;
        }

        return ObtenerTextoVisible(
            _mensajeGlobal
        );
    }

    public string EsperarMensajeNuevo(
        string mensajeAnterior)
    {
        return _espera.Until(_ =>
        {
            var mensajeActual =
                ObtenerMensajeActual();

            if (
                string.IsNullOrWhiteSpace(
                    mensajeActual
                )
            )
            {
                return null;
            }

            return !string.Equals(
                mensajeActual,
                mensajeAnterior,
                StringComparison.Ordinal
            )
                ? mensajeActual
                : null;
        })!;
    }

    public void EliminarPorMatricula(
        string matricula)
    {
        Buscar(matricula);

        var fila =
            EsperarEstudianteEnTabla(
                matricula
            );

        var botonEliminar =
            fila.FindElement(
                By.CssSelector(
                    "button[id^='btn-eliminar-estudiante-']"
                )
            );

        botonEliminar.Click();

        _espera.Until(driver =>
            driver
                .FindElement(_modalEliminar)
                .Displayed
        );

        _driver
            .FindElement(
                _botonConfirmarEliminacion
            )
            .Click();

        EsperarCantidadDeEstudiantes(
            matricula,
            0
        );
    }

    private void Escribir(
        By localizador,
        string valor)
    {
        var campo =
            _driver.FindElement(localizador);

        campo.Clear();
        campo.SendKeys(valor);
    }

    private string ObtenerTextoVisible(
        By localizador)
    {
        try
        {
            var elemento =
                _driver.FindElement(
                    localizador
                );

            return elemento.Displayed
                ? elemento.Text.Trim()
                : string.Empty;
        }
        catch (NoSuchElementException)
        {
            return string.Empty;
        }
        catch (StaleElementReferenceException)
        {
            return string.Empty;
        }
    }
}
