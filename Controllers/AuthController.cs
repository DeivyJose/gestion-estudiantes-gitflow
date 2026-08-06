using System.Security.Claims;
using GestionEstudiantesApi.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionEstudiantesApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> IniciarSesion(LoginDto loginDto)
    {
        var usuarioConfigurado =
            _configuration["DemoUser:Username"];

        var contrasenaConfigurada =
            _configuration["DemoUser:Password"];

        if (string.IsNullOrWhiteSpace(usuarioConfigurado) ||
            string.IsNullOrWhiteSpace(contrasenaConfigurada))
        {
            return StatusCode(500, new
            {
                mensaje = "Las credenciales de demostración no están configuradas."
            });
        }

        var usuarioValido = string.Equals(
            loginDto.Usuario.Trim(),
            usuarioConfigurado,
            StringComparison.Ordinal
        );

        var contrasenaValida = string.Equals(
            loginDto.Contrasena,
            contrasenaConfigurada,
            StringComparison.Ordinal
        );

        if (!usuarioValido || !contrasenaValida)
        {
            return Unauthorized(new
            {
                mensaje = "El usuario o la contraseña son incorrectos."
            });
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, usuarioConfigurado),
            new(ClaimTypes.Role, "Administrador")
        };

        var identidad = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        var propiedades = new AuthenticationProperties
        {
            IsPersistent = false,
            AllowRefresh = true
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identidad),
            propiedades
        );

        return Ok(new
        {
            mensaje = "Inicio de sesión realizado correctamente.",
            usuario = usuarioConfigurado
        });
    }

    [Authorize]
    [HttpGet("status")]
    public IActionResult ObtenerEstado()
    {
        return Ok(new
        {
            autenticado = true,
            usuario = User.Identity?.Name,
            rol = User.FindFirstValue(ClaimTypes.Role)
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> CerrarSesion()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        return Ok(new
        {
            mensaje = "La sesión fue cerrada correctamente."
        });
    }
}
