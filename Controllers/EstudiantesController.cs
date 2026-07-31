using GestionEstudiantesApi.Data;
using GestionEstudiantesApi.DTOs;
using GestionEstudiantesApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionEstudiantesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstudiantesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EstudiantesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EstudianteDto>>> ObtenerTodos()
    {
        var estudiantes = await _context.Estudiantes
            .AsNoTracking()
            .OrderBy(estudiante => estudiante.Apellidos)
            .ThenBy(estudiante => estudiante.Nombres)
            .Select(estudiante => new EstudianteDto
            {
                Id = estudiante.Id,
                Matricula = estudiante.Matricula,
                Nombres = estudiante.Nombres,
                Apellidos = estudiante.Apellidos,
                Correo = estudiante.Correo,
                Carrera = estudiante.Carrera,
                FechaNacimiento = estudiante.FechaNacimiento,
                Activo = estudiante.Activo,
                FechaRegistro = estudiante.FechaRegistro
            })
            .ToListAsync();

        return Ok(estudiantes);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EstudianteDto>> ObtenerPorId(int id)
    {
        var estudiante = await _context.Estudiantes
            .AsNoTracking()
            .Where(estudiante => estudiante.Id == id)
            .Select(estudiante => new EstudianteDto
            {
                Id = estudiante.Id,
                Matricula = estudiante.Matricula,
                Nombres = estudiante.Nombres,
                Apellidos = estudiante.Apellidos,
                Correo = estudiante.Correo,
                Carrera = estudiante.Carrera,
                FechaNacimiento = estudiante.FechaNacimiento,
                Activo = estudiante.Activo,
                FechaRegistro = estudiante.FechaRegistro
            })
            .FirstOrDefaultAsync();

        if (estudiante is null)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró un estudiante con el identificador {id}."
            });
        }

        return Ok(estudiante);
    }

    [HttpPost]
public async Task<ActionResult<EstudianteDto>> Crear(
    CrearEstudianteDto estudianteDto)
{
    var matricula = estudianteDto.Matricula.Trim();
    var correo = estudianteDto.Correo.Trim().ToLowerInvariant();

    var matriculaRegistrada = await _context.Estudiantes
        .AnyAsync(estudiante => estudiante.Matricula == matricula);

    if (matriculaRegistrada)
    {
        return Conflict(new
        {
            mensaje = $"Ya existe un estudiante con la matrícula {matricula}."
        });
    }

    var correoRegistrado = await _context.Estudiantes
        .AnyAsync(estudiante => estudiante.Correo == correo);

    if (correoRegistrado)
    {
        return Conflict(new
        {
            mensaje = $"Ya existe un estudiante con el correo {correo}."
        });
    }

    var estudiante = new Estudiante
    {
        Matricula = matricula,
        Nombres = estudianteDto.Nombres.Trim(),
        Apellidos = estudianteDto.Apellidos.Trim(),
        Correo = correo,
        Carrera = estudianteDto.Carrera.Trim(),
        FechaNacimiento = DateTime.SpecifyKind(
            estudianteDto.FechaNacimiento!.Value,
            DateTimeKind.Utc
        ),
        Activo = true,
        FechaRegistro = DateTime.UtcNow
    };

    _context.Estudiantes.Add(estudiante);
    await _context.SaveChangesAsync();

    var respuesta = ConvertirADto(estudiante);

    return CreatedAtAction(
        nameof(ObtenerPorId),
        new { id = estudiante.Id },
        respuesta
    );
}

    [HttpPut("{id:int}")]
public async Task<ActionResult<EstudianteDto>> Actualizar(
    int id,
    ActualizarEstudianteDto estudianteDto)
{
    var estudiante = await _context.Estudiantes.FindAsync(id);

    if (estudiante is null)
    {
        return NotFound(new
        {
            mensaje = $"No se encontró un estudiante con el identificador {id}."
        });
    }

    var matricula = estudianteDto.Matricula.Trim();
    var correo = estudianteDto.Correo.Trim().ToLowerInvariant();

    var matriculaRegistrada = await _context.Estudiantes
        .AnyAsync(otroEstudiante =>
            otroEstudiante.Id != id &&
            otroEstudiante.Matricula == matricula
        );

    if (matriculaRegistrada)
    {
        return Conflict(new
        {
            mensaje = $"Ya existe otro estudiante con la matrícula {matricula}."
        });
    }

    var correoRegistrado = await _context.Estudiantes
        .AnyAsync(otroEstudiante =>
            otroEstudiante.Id != id &&
            otroEstudiante.Correo == correo
        );

    if (correoRegistrado)
    {
        return Conflict(new
        {
            mensaje = $"Ya existe otro estudiante con el correo {correo}."
        });
    }

    estudiante.Matricula = matricula;
    estudiante.Nombres = estudianteDto.Nombres.Trim();
    estudiante.Apellidos = estudianteDto.Apellidos.Trim();
    estudiante.Correo = correo;
    estudiante.Carrera = estudianteDto.Carrera.Trim();
    estudiante.FechaNacimiento = DateTime.SpecifyKind(
        estudianteDto.FechaNacimiento!.Value,
        DateTimeKind.Utc
    );
    estudiante.Activo = estudianteDto.Activo;

    await _context.SaveChangesAsync();

    return Ok(ConvertirADto(estudiante));
}

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var estudiante = await _context.Estudiantes.FindAsync(id);

        if (estudiante is null)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró un estudiante con el identificador {id}."
            });
        }

        _context.Estudiantes.Remove(estudiante);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static EstudianteDto ConvertirADto(Estudiante estudiante)
    {
        return new EstudianteDto
        {
            Id = estudiante.Id,
            Matricula = estudiante.Matricula,
            Nombres = estudiante.Nombres,
            Apellidos = estudiante.Apellidos,
            Correo = estudiante.Correo,
            Carrera = estudiante.Carrera,
            FechaNacimiento = estudiante.FechaNacimiento,
            Activo = estudiante.Activo,
            FechaRegistro = estudiante.FechaRegistro
        };
    }
}