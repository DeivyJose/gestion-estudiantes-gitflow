using GestionEstudiantes.SeleniumTests.Utilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace GestionEstudiantes.SeleniumTests.Pages;

public sealed class EdicionEstudiantesPage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _espera;

    private readonly By _campoBusqueda =
        By.Id("buscar-estudiante");

    private readonly By _filasTabla =
        By.CssSelector(
            "#cuerpo-tabla-estudiantes tr"
        );

    private readonly By _estudianteId =
        By.Id("estudiante-id");

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

    private readonly By _botonCancelar =
        By.Id("btn-cancelar-edicion");

    private readonly By _mensajeFormulario =
        By.Id("mensaje-formulario");

    private readonly By _mensajeGlobal =
        By.Id("mensaje-global");

    public EdicionEstudiantesPage(
        IWebDriver driver,
        WebDriverWait espera)
    {
        _driver = driver;
        _espera = espera;
    }

    public void EditarPorMatricula(
        string matricula)
    {
        Buscar(matricula);

        var fila = EsperarFilaConTexto(
            matricula
        );

        var botonEditar = fila.FindElement(
            By.CssSelector(
                "button[id^='btn-editar-estudiante-']"
            )
        );

        botonEditar.Click();

        _espera.Until(driver =>
        {
            var id = driver
                .FindElement(_estudianteId)
                .GetAttribute("value");

            return !string.IsNullOrWhiteSpace(id);
        });

        _espera.Until(driver =>
            driver
                .FindElement(_botonCancelar)
                .Displayed
        );

        _espera.Until(driver =>
        {
            var valorMatricula = driver
                .FindElement(_matricula)
                .GetAttribute("value");

            return string.Equals(
                valorMatricula,
                matricula,
                StringComparison.Ordinal
            );
        });
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

    public void GuardarCambios()
    {
        _driver
            .FindElement(_botonGuardar)
            .Click();
    }

    public void Actualizar(
        EstudianteTestData estudiante)
    {
        CompletarFormulario(estudiante);
        GuardarCambios();
    }

    public IWebElement EsperarFilaActualizada(
        string matricula,
        string textoEsperado)
    {
        Buscar(matricula);

        return _espera.Until(driver =>
        {
            try
            {
                return driver
                    .FindElements(_filasTabla)
                    .FirstOrDefault(fila =>
                        fila.Displayed &&
                        fila.Text.Contains(
                            matricula,
                            StringComparison.OrdinalIgnoreCase
                        ) &&
                        fila.Text.Contains(
                            textoEsperado,
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

    public IWebElement EsperarFilaConTexto(
        string texto)
    {
        return _espera.Until(driver =>
        {
            try
            {
                return driver
                    .FindElements(_filasTabla)
                    .FirstOrDefault(fila =>
                        fila.Displayed &&
                        fila.Text.Contains(
                            texto,
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

    public int ObtenerMaximoCarrera()
    {
        var valor = _driver
            .FindElement(_carrera)
            .GetAttribute("maxlength");

        if (
            string.IsNullOrWhiteSpace(valor) ||
            !int.TryParse(valor, out var maximo)
        )
        {
            throw new InvalidOperationException(
                "El campo carrera no posee un maxlength válido."
            );
        }

        return maximo;
    }

    public int ObtenerLongitudCarrera()
    {
        var valor = _driver
            .FindElement(_carrera)
            .GetAttribute("value")
            ?? string.Empty;

        return valor.Length;
    }

    private void Buscar(
        string texto)
    {
        var campo =
            _driver.FindElement(
                _campoBusqueda
            );

        campo.Clear();
        campo.SendKeys(texto);
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
