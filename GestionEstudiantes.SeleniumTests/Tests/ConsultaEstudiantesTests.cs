using GestionEstudiantes.SeleniumTests.Base;
using GestionEstudiantes.SeleniumTests.Pages;
using GestionEstudiantes.SeleniumTests.Utilities;

namespace GestionEstudiantes.SeleniumTests.Tests;

[TestFixture]
[NonParallelizable]
public class ConsultaEstudiantesTests
    : SeleniumTestBase
{
    private EstudiantesPage
        _paginaEstudiantes = null!;

    private ConsultaEstudiantesPage
        _paginaConsulta = null!;

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

        _paginaConsulta =
            new ConsultaEstudiantesPage(
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
    public void ConsultarEstudiante_MatriculaExistente_DebeMostrarRegistro()
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

        _paginaConsulta
            .Buscar(estudiante.Matricula);

        var fila =
            _paginaConsulta
                .EsperarFilaConTexto(
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
                _paginaConsulta
                    .ContarFilasConTexto(
                        estudiante.Matricula
                    ),
                Is.EqualTo(1),
                "La búsqueda debería mostrar un solo registro."
            );
        });
    }

    [Test]
    public void ConsultarEstudiante_DatoInexistente_DebeMostrarSinResultados()
    {
        var busquedaInexistente =
            $"NO-EXISTE-{Guid.NewGuid():N}";

        _paginaConsulta
            .Buscar(busquedaInexistente);

        _paginaConsulta
            .EsperarSinResultados();

        Assert.Multiple(() =>
        {
            Assert.That(
                _paginaConsulta
                    .EstaVisibleSinResultados(),
                Is.True,
                "No se mostró el mensaje de búsqueda sin resultados."
            );

            Assert.That(
                _paginaConsulta
                    .ContarFilasConTexto(
                        busquedaInexistente
                    ),
                Is.EqualTo(0),
                "La tabla mostró registros para un dato inexistente."
            );
        });
    }

    [Test]
    public void ConsultarEstudiante_TextoSobreElLimite_DebeRespetarMaxlength()
    {
        var maximo =
            _paginaConsulta
                .ObtenerMaximoBusqueda();

        var textoExcesivo =
            new string(
                'x',
                maximo + 25
            );

        _paginaConsulta
            .Buscar(textoExcesivo);

        _paginaConsulta
            .EsperarSinResultados();

        Assert.Multiple(() =>
        {
            Assert.That(
                _paginaConsulta
                    .ObtenerLongitudBusqueda(),
                Is.EqualTo(maximo),
                "El campo de búsqueda permitió superar su maxlength."
            );

            Assert.That(
                _paginaConsulta
                    .EstaVisibleSinResultados(),
                Is.True,
                "La interfaz no manejó correctamente la búsqueda límite."
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
