using GestionEstudiantes.SeleniumTests.Base;
using GestionEstudiantes.SeleniumTests.Pages;
using GestionEstudiantes.SeleniumTests.Utilities;

namespace GestionEstudiantes.SeleniumTests.Tests;

[TestFixture]
[NonParallelizable]
public class EliminacionEstudiantesTests
    : SeleniumTestBase
{
    private EstudiantesPage
        _paginaEstudiantes = null!;

    private EliminacionEstudiantesPage
        _paginaEliminacion = null!;

    private readonly List<string>
        _matriculasParaLimpiar = new();

    [SetUp]
    public void IniciarSesion()
    {
        _matriculasParaLimpiar.Clear();

        var paginaLogin = new LoginPage(
            Driver,
            Espera
        );

        paginaLogin.Abrir(UrlBase);

        paginaLogin.IniciarSesion(
            TestSettings.UsuarioValido,
            TestSettings.ContrasenaValida
        );

        paginaLogin.EsperarIngresoExitoso();

        _paginaEstudiantes =
            new EstudiantesPage(
                Driver,
                Espera
            );

        _paginaEstudiantes
            .EsperarPaginaCargada();

        _paginaEliminacion =
            new EliminacionEstudiantesPage(
                Driver,
                Espera
            );
    }

    [TearDown]
    public void LimpiarDatosRestantes()
    {
        foreach (
            var matricula in
            _matriculasParaLimpiar
                .Distinct()
                .Reverse()
        )
        {
            try
            {
                _paginaEstudiantes
                    .EliminarPorMatricula(
                        matricula
                    );
            }
            catch (Exception error)
            {
                TestContext.Progress.WriteLine(
                    $"No fue necesario o no fue posible limpiar " +
                    $"{matricula}: {error.Message}"
                );
            }
        }
    }

    [Test]
    public void EliminarEstudiante_ConfirmarEliminacion_DebeDesaparecerDeTabla()
    {
        var estudiante =
            EstudianteTestDataFactory
                .CrearValido();

        RegistrarParaLimpieza(
            estudiante.Matricula
        );

        _paginaEstudiantes
            .Registrar(estudiante);

        _paginaEstudiantes
            .EsperarEstudianteEnTabla(
                estudiante.Matricula
            );

        _paginaEliminacion
            .AbrirModalPorMatricula(
                estudiante.Matricula
            );

        var nombreMostrado =
            _paginaEliminacion
                .ObtenerNombreMostradoEnModal();

        Assert.That(
            nombreMostrado,
            Does.Contain(
                estudiante.Nombres
            ).IgnoreCase,
            "El modal no mostró el estudiante seleccionado."
        );

        _paginaEliminacion
            .ConfirmarEliminacion();

        _paginaEliminacion
            .EsperarEstudianteAusente(
                estudiante.Matricula
            );

        _matriculasParaLimpiar.Remove(
            estudiante.Matricula
        );

        Assert.Multiple(() =>
        {
            Assert.That(
                _paginaEliminacion
                    .ExisteEstudiante(
                        estudiante.Matricula
                    ),
                Is.False,
                "El estudiante todavía aparece en la tabla."
            );

            Assert.That(
                _paginaEliminacion
                    .EstaVisibleSinResultados(),
                Is.True,
                "No se mostró el estado sin resultados."
            );
        });
    }

    [Test]
    public void EliminarEstudiante_CancelarEliminacion_DebeConservarRegistro()
    {
        var estudiante =
            EstudianteTestDataFactory
                .CrearValido();

        RegistrarParaLimpieza(
            estudiante.Matricula
        );

        _paginaEstudiantes
            .Registrar(estudiante);

        _paginaEstudiantes
            .EsperarEstudianteEnTabla(
                estudiante.Matricula
            );

        _paginaEliminacion
            .AbrirModalPorMatricula(
                estudiante.Matricula
            );

        Assert.That(
            _paginaEliminacion
                .EstaVisibleElModal(),
            Is.True,
            "El modal de confirmación no se abrió."
        );

        _paginaEliminacion
            .CancelarEliminacion();

        var fila =
            _paginaEliminacion
                .EsperarFilaPorMatricula(
                    estudiante.Matricula
                );

        Assert.Multiple(() =>
        {
            Assert.That(
                _paginaEliminacion
                    .EstaVisibleElModal(),
                Is.False,
                "El modal continuó visible después de cancelar."
            );

            Assert.That(
                fila.Text,
                Does.Contain(
                    estudiante.Matricula
                ),
                "El registro desapareció aunque se canceló la eliminación."
            );
        });
    }

    [Test]
    public void EliminarEstudiante_NombreEnLimiteMaximo_DebeEliminarCorrectamente()
    {
        var estudianteBase =
            EstudianteTestDataFactory
                .CrearValido();

        var nombreLimite =
            new string(
                'N',
                100
            );

        var estudiante =
            estudianteBase with
            {
                Nombres = nombreLimite
            };

        RegistrarParaLimpieza(
            estudiante.Matricula
        );

        _paginaEstudiantes
            .Registrar(estudiante);

        _paginaEstudiantes
            .EsperarEstudianteEnTabla(
                estudiante.Matricula
            );

        _paginaEliminacion
            .AbrirModalPorMatricula(
                estudiante.Matricula
            );

        var nombreMostrado =
            _paginaEliminacion
                .ObtenerNombreMostradoEnModal();

        Assert.Multiple(() =>
        {
            Assert.That(
                estudiante.Nombres.Length,
                Is.EqualTo(100),
                "El nombre de prueba no posee el límite esperado."
            );

            Assert.That(
                nombreMostrado,
                Does.Contain(nombreLimite),
                "El modal no mostró correctamente el nombre límite."
            );
        });

        _paginaEliminacion
            .ConfirmarEliminacion();

        _paginaEliminacion
            .EsperarEstudianteAusente(
                estudiante.Matricula
            );

        _matriculasParaLimpiar.Remove(
            estudiante.Matricula
        );

        Assert.That(
            _paginaEliminacion
                .ExisteEstudiante(
                    estudiante.Matricula
                ),
            Is.False,
            "El estudiante con nombre límite no fue eliminado."
        );
    }

    private void RegistrarParaLimpieza(
        string matricula)
    {
        _matriculasParaLimpiar.Add(
            matricula
        );
    }
}
