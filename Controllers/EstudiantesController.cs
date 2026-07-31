using GestionEstudiantesApi.Data;
using GestionEstudiantesApi.DTOs;
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
}