using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace GestionEstudiantes.SeleniumTests.Pages;

public sealed class EliminacionEstudiantesPage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _espera;

    private readonly By _campoBusqueda =
        By.Id("buscar-estudiante");

    private readonly By _filasTabla =
        By.CssSelector(
            "#cuerpo-tabla-estudiantes tr"
        );

    private readonly By _modalEliminar =
        By.Id("modal-eliminar");

    private readonly By _nombreEstudianteEliminar =
        By.Id("nombre-estudiante-eliminar");

    private readonly By _botonConfirmar =
        By.Id("btn-confirmar-eliminacion");

    private readonly By _botonCancelar =
        By.Id("btn-cancelar-eliminacion");

    private readonly By _sinResultados =
        By.Id("sin-resultados");

    public EliminacionEstudiantesPage(
        IWebDriver driver,
        WebDriverWait espera)
    {
        _driver = driver;
        _espera = espera;
    }

    public void AbrirModalPorMatricula(
        string matricula)
    {
        Buscar(matricula);

        var fila =
            EsperarFilaPorMatricula(
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
        {
            try
            {
                return driver
                    .FindElement(_modalEliminar)
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
        });
    }

    public void ConfirmarEliminacion()
    {
        _driver
            .FindElement(_botonConfirmar)
            .Click();

        EsperarModalOculto();
    }

    public void CancelarEliminacion()
    {
        _driver
            .FindElement(_botonCancelar)
            .Click();

        EsperarModalOculto();
    }

    public string ObtenerNombreMostradoEnModal()
    {
        return _driver
            .FindElement(
                _nombreEstudianteEliminar
            )
            .Text
            .Trim();
    }

    public bool EstaVisibleElModal()
    {
        try
        {
            return _driver
                .FindElement(_modalEliminar)
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

    public IWebElement EsperarFilaPorMatricula(
        string matricula)
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

    public void EsperarEstudianteAusente(
        string matricula)
    {
        Buscar(matricula);

        _espera.Until(driver =>
        {
            try
            {
                var existe = driver
                    .FindElements(_filasTabla)
                    .Any(fila =>
                        fila.Displayed &&
                        fila.Text.Contains(
                            matricula,
                            StringComparison.OrdinalIgnoreCase
                        )
                    );

                return !existe;
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
        });
    }

    public bool ExisteEstudiante(
        string matricula)
    {
        return _driver
            .FindElements(_filasTabla)
            .Any(fila =>
                fila.Displayed &&
                fila.Text.Contains(
                    matricula,
                    StringComparison.OrdinalIgnoreCase
                )
            );
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

    private void Buscar(string texto)
    {
        var campo =
            _driver.FindElement(
                _campoBusqueda
            );

        campo.Clear();
        campo.SendKeys(texto);
    }

    private void EsperarModalOculto()
    {
        _espera.Until(driver =>
        {
            try
            {
                return !driver
                    .FindElement(_modalEliminar)
                    .Displayed;
            }
            catch (NoSuchElementException)
            {
                return true;
            }
            catch (StaleElementReferenceException)
            {
                return true;
            }
        });
    }
}
