using GestionEstudiantesApi.Data;
using Microsoft.AspNetCore.Mvc;

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
}