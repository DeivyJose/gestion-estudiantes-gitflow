using GestionEstudiantes.SeleniumTests.Base;
using GestionEstudiantes.SeleniumTests.Pages;
using GestionEstudiantes.SeleniumTests.Utilities;

namespace GestionEstudiantes.SeleniumTests.Tests;

[TestFixture]
[NonParallelizable]
public class RegistroEstudiantesTests
    : SeleniumTestBase
{
    private LoginPage _paginaLogin = null!;

    private EstudiantesPage
        _paginaEstudiantes = null!;

    private readonly List<string>
        _matriculasParaLimpiar = new();

    [SetUp]
    public void IniciarSesion()
    {
        _matriculasParaLimpiar.Clear();

        _paginaLogin = new LoginPage(
            Driver,
            Espera
        );

        _paginaLogin.Abrir(UrlBase);

        _paginaLogin.IniciarSesion(
            TestSettings.UsuarioValido,
            TestSettings.ContrasenaValida
        );

        _paginaLogin.EsperarIngresoExitoso();

        _paginaEstudiantes =
            new EstudiantesPage(
                Driver,
                Espera
            );

        _paginaEstudiantes
            .EsperarPaginaCargada();
    }

    [TearDown]
    public void LimpiarEstudiantesCreados()
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
                    $"No fue posible limpiar la matrícula " +
                    $"{matricula}: {error.Message}"
                );
            }
        }
    }

    [Test]
    public void RegistrarEstudiante_DatosValidos_DebeAparecerEnLaTabla()
    {
        var estudiante =
            EstudianteTestDataFactory
                .CrearValido();

        RegistrarParaLimpieza(
            estudiante.Matricula
        );

        _paginaEstudiantes
            .Registrar(estudiante);

        var fila =
            _paginaEstudiantes
                .EsperarEstudianteEnTabla(
                    estudiante.Matricula
                );

        Assert.Multiple(() =>
        {
            Assert.That(
                fila.Text,
                Does.Contain(
                    estudiante.Matricula
                )
            );

            Assert.That(
                fila.Text,
                Does.Contain(
                    estudiante.Nombres
                ).IgnoreCase
            );

            Assert.That(
                fila.Text,
                Does.Contain(
                    estudiante.Carrera
                ).IgnoreCase
            );
        });
    }

    [Test]
    public void RegistrarEstudiante_MatriculaDuplicada_DebeSerRechazada()
    {
        var estudianteOriginal =
            EstudianteTestDataFactory
                .CrearValido();

        RegistrarParaLimpieza(
            estudianteOriginal.Matricula
        );

        _paginaEstudiantes
            .Registrar(estudianteOriginal);

        _paginaEstudiantes
            .EsperarEstudianteEnTabla(
                estudianteOriginal.Matricula
            );

        var mensajeAnterior =
            _paginaEstudiantes
                .ObtenerMensajeActual();

        _paginaEstudiantes
            .LimpiarFormulario();

        var estudianteDuplicado =
            EstudianteTestDataFactory
                .CrearConMatriculaDuplicada(
                    estudianteOriginal
                );

        _paginaEstudiantes
            .Registrar(estudianteDuplicado);

        var mensajeError =
            _paginaEstudiantes
                .EsperarMensajeNuevo(
                    mensajeAnterior
                );

        _paginaEstudiantes
            .Buscar(
                estudianteOriginal.Matricula
            );

        _paginaEstudiantes
            .EsperarCantidadDeEstudiantes(
                estudianteOriginal.Matricula,
                1
            );

        Assert.Multiple(() =>
        {
            Assert.That(
                mensajeError,
                Is.Not.Empty,
                "No se mostró un mensaje de rechazo."
            );

            Assert.That(
                _paginaEstudiantes
                    .ContarEstudiantes(
                        estudianteOriginal.Matricula
                    ),
                Is.EqualTo(1),
                "La matrícula duplicada fue registrada."
            );
        });
    }

    [Test]
    public void RegistrarEstudiante_ValoresMinimos_DebeSerAceptado()
    {
        var estudiante =
            EstudianteTestDataFactory
                .CrearConValoresMinimos();

        RegistrarParaLimpieza(
            estudiante.Matricula
        );

        _paginaEstudiantes
            .Registrar(estudiante);

        var fila =
            _paginaEstudiantes
                .EsperarEstudianteEnTabla(
                    estudiante.Matricula
                );

        Assert.Multiple(() =>
        {
            Assert.That(
                estudiante.Matricula.Length,
                Is.EqualTo(8)
            );

            Assert.That(
                estudiante.Nombres.Length,
                Is.EqualTo(2)
            );

            Assert.That(
                estudiante.Apellidos.Length,
                Is.EqualTo(2)
            );

            Assert.That(
                estudiante.Carrera.Length,
                Is.EqualTo(2)
            );

            Assert.That(
                fila.Text,
                Does.Contain(
                    estudiante.Matricula
                )
            );
        });
    }

    private void RegistrarParaLimpieza(
        string matricula)
    {
        _matriculasParaLimpiar.Add(
            matricula
        );
    }
}
