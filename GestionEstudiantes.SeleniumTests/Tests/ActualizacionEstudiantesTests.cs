using GestionEstudiantes.SeleniumTests.Base;
using GestionEstudiantes.SeleniumTests.Pages;
using GestionEstudiantes.SeleniumTests.Utilities;

namespace GestionEstudiantes.SeleniumTests.Tests;

[TestFixture]
[NonParallelizable]
public class ActualizacionEstudiantesTests
    : SeleniumTestBase
{
    private EstudiantesPage
        _paginaEstudiantes = null!;

    private EdicionEstudiantesPage
        _paginaEdicion = null!;

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

        _paginaEdicion =
            new EdicionEstudiantesPage(
                Driver,
                Espera
            );
    }

    [TearDown]
    public void LimpiarDatosDePrueba()
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
    public void ActualizarEstudiante_CarreraValida_DebeMostrarCambioEnTabla()
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

        var estudianteActualizado =
            estudiante with
            {
                Carrera =
                    "Ingenieria de Software"
            };

        _paginaEdicion
            .EditarPorMatricula(
                estudiante.Matricula
            );

        _paginaEdicion
            .Actualizar(
                estudianteActualizado
            );

        var fila =
            _paginaEdicion
                .EsperarFilaActualizada(
                    estudiante.Matricula,
                    estudianteActualizado.Carrera
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
                    estudianteActualizado.Carrera
                ).IgnoreCase
            );

            Assert.That(
                fila.Text,
                Does.Not.Contain(
                    estudiante.Carrera
                ).IgnoreCase
            );
        });
    }

    [Test]
    public void ActualizarEstudiante_CorreoDuplicado_DebeSerRechazado()
    {
        var primerEstudiante =
            EstudianteTestDataFactory
                .CrearValido();

        var segundoEstudiante =
            EstudianteTestDataFactory
                .CrearValido();

        RegistrarParaLimpieza(
            primerEstudiante.Matricula
        );

        RegistrarParaLimpieza(
            segundoEstudiante.Matricula
        );

        _paginaEstudiantes
            .Registrar(primerEstudiante);

        _paginaEstudiantes
            .EsperarEstudianteEnTabla(
                primerEstudiante.Matricula
            );

        _paginaEstudiantes
            .LimpiarFormulario();

        _paginaEstudiantes
            .Registrar(segundoEstudiante);

        _paginaEstudiantes
            .EsperarEstudianteEnTabla(
                segundoEstudiante.Matricula
            );

        _paginaEdicion
            .EditarPorMatricula(
                segundoEstudiante.Matricula
            );

        var mensajeAnterior =
            _paginaEdicion
                .ObtenerMensajeActual();

        var actualizacionInvalida =
            segundoEstudiante with
            {
                Correo =
                    primerEstudiante.Correo
            };

        _paginaEdicion
            .Actualizar(
                actualizacionInvalida
            );

        var mensajeError =
            _paginaEdicion
                .EsperarMensajeNuevo(
                    mensajeAnterior
                );

        var fila =
            _paginaEdicion
                .EsperarFilaActualizada(
                    segundoEstudiante.Matricula,
                    segundoEstudiante.Correo
                );

        Assert.Multiple(() =>
        {
            Assert.That(
                mensajeError,
                Is.Not.Empty,
                "No se mostró un mensaje de rechazo."
            );

            Assert.That(
                fila.Text,
                Does.Contain(
                    segundoEstudiante.Correo
                ).IgnoreCase,
                "El correo original fue modificado."
            );

            Assert.That(
                fila.Text,
                Does.Not.Contain(
                    primerEstudiante.Correo
                ).IgnoreCase,
                "El sistema permitió utilizar un correo duplicado."
            );
        });
    }

    [Test]
    public void ActualizarEstudiante_CarreraEnLimiteMaximo_DebeSerAceptada()
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

        _paginaEdicion
            .EditarPorMatricula(
                estudiante.Matricula
            );

        var maximoCarrera =
            _paginaEdicion
                .ObtenerMaximoCarrera();

        var carreraLimite =
            new string(
                'A',
                maximoCarrera
            );

        var estudianteActualizado =
            estudiante with
            {
                Carrera = carreraLimite
            };

        _paginaEdicion
            .CompletarFormulario(
                estudianteActualizado
            );

        Assert.That(
            _paginaEdicion
                .ObtenerLongitudCarrera(),
            Is.EqualTo(maximoCarrera),
            "El campo carrera no aceptó exactamente su límite máximo."
        );

        _paginaEdicion
            .GuardarCambios();

        var fila =
            _paginaEdicion
                .EsperarFilaActualizada(
                    estudiante.Matricula,
                    carreraLimite
                );

        Assert.Multiple(() =>
        {
            Assert.That(
                maximoCarrera,
                Is.EqualTo(150),
                "El límite esperado para la carrera es de 150 caracteres."
            );

            Assert.That(
                fila.Text,
                Does.Contain(
                    carreraLimite
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
