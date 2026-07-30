using GestionEstudiantesApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionEstudiantesApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Estudiante> Estudiantes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Estudiante>(entity =>
        {
            entity.ToTable("Estudiantes");

            entity.HasKey(estudiante => estudiante.Id);

            entity.Property(estudiante => estudiante.Matricula)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(estudiante => estudiante.Nombres)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(estudiante => estudiante.Apellidos)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(estudiante => estudiante.Correo)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(estudiante => estudiante.Carrera)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(estudiante => estudiante.FechaNacimiento)
                .IsRequired();

            entity.Property(estudiante => estudiante.Activo)
                .HasDefaultValue(true);

            entity.Property(estudiante => estudiante.FechaRegistro)
                .IsRequired();

            entity.HasIndex(estudiante => estudiante.Matricula)
                .IsUnique();

            entity.HasIndex(estudiante => estudiante.Correo)
                .IsUnique();
        });
    }
}