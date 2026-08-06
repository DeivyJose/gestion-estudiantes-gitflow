using GestionEstudiantes.SeleniumTests.Reporting;

namespace GestionEstudiantes.SeleniumTests;

[SetUpFixture]
public sealed class ReporteGlobalSetup
{
    [OneTimeSetUp]
    public void InicializarReporte()
    {
        ExtentReportManager.Inicializar();

        TestContext.Progress.WriteLine(
            "Reporte HTML inicializado."
        );
    }

    [OneTimeTearDown]
    public void FinalizarReporte()
    {
        ExtentReportManager.Finalizar();

        TestContext.Progress.WriteLine(
            $"Reporte generado en: " +
            $"{ExtentReportManager.RutaReporte}"
        );
    }
}
