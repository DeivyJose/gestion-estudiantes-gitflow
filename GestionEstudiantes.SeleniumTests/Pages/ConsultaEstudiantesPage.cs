using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace GestionEstudiantes.SeleniumTests.Pages;

public sealed class ConsultaEstudiantesPage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _espera;

    private readonly By _campoBusqueda =
        By.Id("buscar-estudiante");

    private readonly By _filasTabla =
        By.CssSelector(
            "#cuerpo-tabla-estudiantes tr"
        );

    private readonly By _sinResultados =
        By.Id("sin-resultados");

    private readonly By _estadoCarga =
        By.Id("estado-carga");

    private readonly By _botonLimpiarBusqueda =
        By.Id("btn-limpiar-busqueda");

    public ConsultaEstudiantesPage(
        IWebDriver driver,
        WebDriverWait espera)
    {
        _driver = driver;
        _espera = espera;
    }

    public void Buscar(string texto)
    {
        var campo =
            _driver.FindElement(
                _campoBusqueda
            );

        campo.Clear();
        campo.SendKeys(texto);

        EsperarFinDeBusqueda();
    }

    public void LimpiarBusqueda()
    {
        var boton =
            _driver.FindElement(
                _botonLimpiarBusqueda
            );

        boton.Click();

        _espera.Until(driver =>
        {
            var valor = driver
                .FindElement(_campoBusqueda)
                .GetAttribute("value");

            return string.IsNullOrEmpty(valor);
        });

        EsperarFinDeBusqueda();
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

    public void EsperarSinResultados()
    {
        _espera.Until(driver =>
        {
            try
            {
                var elemento =
                    driver.FindElement(
                        _sinResultados
                    );

                return elemento.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
        });
    }

    public bool EstaVisibleSinResultados()
    {
        try
        {
            return _driver
                .FindElement(_sinResultados)
                .Displayed;
        }
        catch (NoSuchElementException)
        {
            return false;
        }
        catch (StaleElementReferenceException)
        {
            return false;
        }
    }

    public int ContarFilasConTexto(
        string texto)
    {
        return _driver
            .FindElements(_filasTabla)
            .Count(fila =>
                fila.Displayed &&
                fila.Text.Contains(
                    texto,
                    StringComparison.OrdinalIgnoreCase
                )
            );
    }

    public int ObtenerMaximoBusqueda()
    {
        var valor = _driver
            .FindElement(_campoBusqueda)
            .GetAttribute("maxlength");

        if (
            string.IsNullOrWhiteSpace(valor) ||
            !int.TryParse(valor, out var maximo)
        )
        {
            throw new InvalidOperationException(
                "El campo de búsqueda no posee un maxlength válido."
            );
        }

        return maximo;
    }

    public int ObtenerLongitudBusqueda()
    {
        var valor = _driver
            .FindElement(_campoBusqueda)
            .GetAttribute("value")
            ?? string.Empty;

        return valor.Length;
    }

    private void EsperarFinDeBusqueda()
    {
        _espera.Until(driver =>
        {
            try
            {
                var estado =
                    driver.FindElement(
                        _estadoCarga
                    );

                return !estado.Displayed ||
                       string.IsNullOrWhiteSpace(
                           estado.Text
                       );
            }
            catch (NoSuchElementException)
            {
                return true;
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
        });
    }
}
