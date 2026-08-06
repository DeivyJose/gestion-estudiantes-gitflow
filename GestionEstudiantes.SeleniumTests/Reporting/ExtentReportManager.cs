using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Config;
using NUnit.Framework.Interfaces;

namespace GestionEstudiantes.SeleniumTests.Reporting;

public static class ExtentReportManager
{
    private static readonly object Candado = new();

    private static readonly AsyncLocal<ExtentTest?>
        PruebaActual = new();

    private static ExtentReports? _reporte;

    public static string RutaReporte { get; private set; } =
        string.Empty;

    public static string CarpetaCapturas { get; private set; } =
        string.Empty;

    public static void Inicializar()
    {
        lock (Candado)
        {
            if (_reporte is not null)
            {
                return;
            }

            var raizRepositorio =
                EncontrarRaizRepositorio();

            var carpetaReportes = Path.Combine(
                raizRepositorio,
                "Evidencias",
                "Reportes"
            );

            CarpetaCapturas = Path.Combine(
                raizRepositorio,
                "Evidencias",
                "Capturas"
            );

            Directory.CreateDirectory(
                carpetaReportes
            );

            Directory.CreateDirectory(
                CarpetaCapturas
            );

            RutaReporte = Path.Combine(
                carpetaReportes,
                "Reporte-Selenium.html"
            );

            var reporteHtml =
                new ExtentSparkReporter(
                    RutaReporte
                );

            reporteHtml.Config.DocumentTitle =
                "Reporte de pruebas automatizadas";

            reporteHtml.Config.ReportName =
                "Gestión de Estudiantes - Selenium";

            reporteHtml.Config.Theme =
                Theme.Standard;

            _reporte = new ExtentReports();

            _reporte.AttachReporter(
                reporteHtml
            );

            _reporte.AddSystemInfo(
                "Proyecto",
                "Gestión de Estudiantes"
            );

            _reporte.AddSystemInfo(
                "Framework",
                ".NET 8 + NUnit"
            );

            _reporte.AddSystemInfo(
                "Automatización",
                "Selenium WebDriver"
            );

            _reporte.AddSystemInfo(
                "Navegador",
                "Microsoft Edge"
            );

            _reporte.AddSystemInfo(
                "Sistema operativo",
                Environment.OSVersion.ToString()
            );
        }
    }

    public static void IniciarPrueba(
        string nombre,
        string nombreCompleto)
    {
        Inicializar();

        lock (Candado)
        {
            var prueba = _reporte!
                .CreateTest(nombre);

            prueba.Info(
                $"Prueba: {nombreCompleto}"
            );

            prueba.AssignCategory(
                ObtenerCategoria(
                    nombreCompleto
                )
            );

            PruebaActual.Value = prueba;
        }
    }

    public static void RegistrarResultado(
        TestStatus estado,
        string? mensaje,
        string? traza,
        string? capturaBase64)
    {
        lock (Candado)
        {
            var prueba =
                PruebaActual.Value;

            if (prueba is null)
            {
                return;
            }

            var detalles =
                string.IsNullOrWhiteSpace(mensaje)
                    ? "Sin detalles adicionales."
                    : mensaje;

            switch (estado)
            {
                case TestStatus.Passed:
                    RegistrarAprobada(
                        prueba,
                        capturaBase64
                    );
                    break;

                case TestStatus.Failed:
                    RegistrarFallida(
                        prueba,
                        detalles,
                        traza,
                        capturaBase64
                    );
                    break;

                case TestStatus.Skipped:
                    RegistrarOmitida(
                        prueba,
                        detalles,
                        capturaBase64
                    );
                    break;

                default:
                    prueba.Warning(
                        detalles
                    );
                    break;
            }

            PruebaActual.Value = null;
        }
    }

    public static void Finalizar()
    {
        lock (Candado)
        {
            _reporte?.Flush();
        }
    }

    private static void RegistrarAprobada(
        ExtentTest prueba,
        string? capturaBase64)
    {
        if (
            string.IsNullOrWhiteSpace(
                capturaBase64
            )
        )
        {
            prueba.Pass(
                "Prueba completada correctamente."
            );

            return;
        }

        var captura = MediaEntityBuilder
            .CreateScreenCaptureFromBase64String(
                capturaBase64,
                "Captura final"
            )
            .Build();

        prueba.Pass(
            "Prueba completada correctamente.",
            captura
        );
    }

    private static void RegistrarFallida(
        ExtentTest prueba,
        string detalles,
        string? traza,
        string? capturaBase64)
    {
        if (
            string.IsNullOrWhiteSpace(
                capturaBase64
            )
        )
        {
            prueba.Fail(detalles);
        }
        else
        {
            var captura = MediaEntityBuilder
                .CreateScreenCaptureFromBase64String(
                    capturaBase64,
                    "Captura del error"
                )
                .Build();

            prueba.Fail(
                detalles,
                captura
            );
        }

        if (
            !string.IsNullOrWhiteSpace(
                traza
            )
        )
        {
            prueba.Fail(traza);
        }
    }

    private static void RegistrarOmitida(
        ExtentTest prueba,
        string detalles,
        string? capturaBase64)
    {
        if (
            string.IsNullOrWhiteSpace(
                capturaBase64
            )
        )
        {
            prueba.Skip(detalles);
            return;
        }

        var captura = MediaEntityBuilder
            .CreateScreenCaptureFromBase64String(
                capturaBase64,
                "Captura final"
            )
            .Build();

        prueba.Skip(
            detalles,
            captura
        );
    }

    private static string ObtenerCategoria(
        string nombreCompleto)
    {
        if (
            nombreCompleto.Contains(
                "LoginTests",
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            return "Inicio de sesión";
        }

        if (
            nombreCompleto.Contains(
                "RegistroEstudiantesTests",
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            return "Registro";
        }

        if (
            nombreCompleto.Contains(
                "ConsultaEstudiantesTests",
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            return "Consulta";
        }

        if (
            nombreCompleto.Contains(
                "ActualizacionEstudiantesTests",
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            return "Actualización";
        }

        if (
            nombreCompleto.Contains(
                "EliminacionEstudiantesTests",
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            return "Eliminación";
        }

        return "Otras pruebas";
    }

    private static string EncontrarRaizRepositorio()
    {
        var carpetaActual =
            new DirectoryInfo(
                AppContext.BaseDirectory
            );

        while (carpetaActual is not null)
        {
            var solucion = Path.Combine(
                carpetaActual.FullName,
                "GestionEstudiantes.sln"
            );

            if (File.Exists(solucion))
            {
                return carpetaActual.FullName;
            }

            carpetaActual =
                carpetaActual.Parent;
        }

        throw new DirectoryNotFoundException(
            "No se encontró la raíz del repositorio."
        );
    }
}
