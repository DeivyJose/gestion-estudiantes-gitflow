using GestionEstudiantesApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errores = context.ModelState
            .Where(elemento => elemento.Value?.Errors.Count > 0)
            .ToDictionary(
                elemento => elemento.Key,
                elemento => elemento.Value!.Errors
                    .Select(error =>
                        string.IsNullOrWhiteSpace(error.ErrorMessage)
                            ? "El valor enviado no es válido."
                            : error.ErrorMessage
                    )
                    .ToArray()
            );

        return new BadRequestObjectResult(new
        {
            mensaje = "Los datos enviados no son válidos.",
            errores
        });
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration
    .GetConnectionString("PostgreSql");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "No se encontró la cadena de conexión de PostgreSQL."
    );
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();