using System.Text.RegularExpressions;
using GestionEstudiantes.SeleniumTests.Reporting;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

namespace GestionEstudiantes.SeleniumTests.Base;

public abstract class SeleniumTestBase
{
    protected const string UrlBase =
        "http://localhost:5176";

    protected IWebDriver Driver { get; private set; } =
        null!;

    protected WebDriverWait Espera { get; private set; } =
        null!;

    private string? _rutaPerfilTemporal;

    [SetUp]
    public void PrepararNavegador()
    {
        var nombrePrueba =
            TestContext.CurrentContext.Test.Name;

        var nombreCompleto =
            TestContext.CurrentContext.Test.FullName
            ?? nombrePrueba;

        ExtentReportManager.IniciarPrueba(
            nombrePrueba,
            nombreCompleto
        );

        var rutaEdge =
            EncontrarMicrosoftEdge();

        _rutaPerfilTemporal = Path.Combine(
            Path.GetTempPath(),
            "gestion-estudiantes-selenium",
            Guid.NewGuid().ToString("N")
        );

        Directory.CreateDirectory(
            _rutaPerfilTemporal
        );

        var opciones = new EdgeOptions
        {
            BinaryLocation = rutaEdge,
            PageLoadStrategy =
                PageLoadStrategy.Eager
        };

        opciones.AddArgument(
            "--no-first-run"
        );

        opciones.AddArgument(
            "--no-default-browser-check"
        );

        opciones.AddArgument(
            "--disable-dev-shm-usage"
        );

        opciones.AddArgument(
            "--disable-extensions"
        );

        opciones.AddArgument(
            "--window-size=1440,900"
        );

        opciones.AddArgument(
            $"--user-data-dir={_rutaPerfilTemporal}"
        );

        var ejecutarSinInterfaz =
            Environment.GetEnvironmentVariable(
                "SELENIUM_HEADLESS"
            ) == "1";

        if (ejecutarSinInterfaz)
        {
            opciones.AddArgument(
                "--headless=new"
            );
        }

        try
        {
            Driver =
                new EdgeDriver(opciones);
        }
        catch (Exception error)
        {
            throw new InvalidOperationException(
                "No fue posible iniciar Microsoft Edge " +
                "con Selenium.",
                error
            );
        }

        Driver.Manage()
            .Timeouts()
            .PageLoad =
                TimeSpan.FromSeconds(60);

        Driver.Manage()
            .Timeouts()
            .ImplicitWait =
                TimeSpan.Zero;

        Espera = new WebDriverWait(
            Driver,
            TimeSpan.FromSeconds(15)
        );
    }

    [TearDown]
    public void RegistrarEvidenciaYCerrarNavegador()
    {
        var pausaTexto =
            Environment.GetEnvironmentVariable(
                "SELENIUM_DEMO_PAUSE_SECONDS"
            );

        if (
            int.TryParse(
                pausaTexto,
                out var pausaSegundos
            ) &&
            pausaSegundos > 0
        )
        {
            Thread.Sleep(
                TimeSpan.FromSeconds(
                    Math.Min(
                        pausaSegundos,
                        30
                    )
                )
            );
        }

        string? capturaBase64 = null;

        try
        {
            capturaBase64 =
                TomarCapturaFinal();
        }
        catch (Exception error)
        {
            TestContext.Progress.WriteLine(
                $"No se pudo tomar la captura: " +
                $"{error.Message}"
            );
        }

        try
        {
            var resultado =
                TestContext
                    .CurrentContext
                    .Result;

            ExtentReportManager
                .RegistrarResultado(
                    resultado.Outcome.Status,
                    resultado.Message,
                    resultado.StackTrace,
                    capturaBase64
                );
        }
        finally
        {
            try
            {
                Driver?.Quit();
                Driver?.Dispose();
            }
            finally
            {
                EliminarPerfilTemporal();
            }
        }
    }

    protected IWebElement EsperarElementoVisible(
        By localizador)
    {
        return Espera.Until(navegador =>
        {
            try
            {
                var elemento =
                    navegador.FindElement(
                        localizador
                    );

                return elemento.Displayed
                    ? elemento
                    : null;
            }
            catch (NoSuchElementException)
            {
                return null;
            }
            catch (
                StaleElementReferenceException
            )
            {
                return null;
            }
        })!;
    }

    protected void EsperarHastaQue(
        Func<IWebDriver, bool> condicion)
    {
        Espera.Until(condicion);
    }

    private string? TomarCapturaFinal()
    {
        if (Driver is not ITakesScreenshot)
        {
            return null;
        }

        var captura =
            ((ITakesScreenshot)Driver)
                .GetScreenshot();

        var nombrePrueba =
            TestContext.CurrentContext.Test.Name;

        var nombreSeguro =
            Regex.Replace(
                nombrePrueba,
                @"[^a-zA-Z0-9áéíóúÁÉÍÓÚñÑ_-]",
                "_"
            );

        var fecha =
            DateTime.Now.ToString(
                "yyyyMMdd-HHmmss-fff"
            );

        var nombreArchivo =
            $"{fecha}-{nombreSeguro}.png";

        var rutaCaptura = Path.Combine(
            ExtentReportManager
                .CarpetaCapturas,
            nombreArchivo
        );

        captura.SaveAsFile(
            rutaCaptura
        );

        TestContext.AddTestAttachment(
            rutaCaptura,
            "Captura final de Selenium"
        );

        return captura
            .AsBase64EncodedString;
    }

    private static string EncontrarMicrosoftEdge()
    {
        var rutaConfigurada =
            Environment.GetEnvironmentVariable(
                "EDGE_BINARY"
            );

        var rutasPosibles = new[]
        {
            rutaConfigurada,
            "/usr/bin/microsoft-edge-stable",
            "/usr/bin/microsoft-edge",
            "/opt/microsoft/msedge/msedge"
        };

        var rutaEncontrada =
            rutasPosibles
                .Where(ruta =>
                    !string.IsNullOrWhiteSpace(
                        ruta
                    )
                )
                .FirstOrDefault(
                    File.Exists
                );

        if (rutaEncontrada is null)
        {
            throw new FileNotFoundException(
                "No se encontró Microsoft Edge. " +
                "Configura EDGE_BINARY con la ruta " +
                "del navegador."
            );
        }

        return rutaEncontrada;
    }

    private void EliminarPerfilTemporal()
    {
        if (
            string.IsNullOrWhiteSpace(
                _rutaPerfilTemporal
            ) ||
            !Directory.Exists(
                _rutaPerfilTemporal
            )
        )
        {
            return;
        }

        for (
            var intento = 1;
            intento <= 5;
            intento++
        )
        {
            try
            {
                Directory.Delete(
                    _rutaPerfilTemporal,
                    recursive: true
                );

                return;
            }
            catch when (intento < 5)
            {
                Thread.Sleep(300);
            }
            catch
            {
                // La limpieza no cambia el resultado.
            }
        }
    }
}
