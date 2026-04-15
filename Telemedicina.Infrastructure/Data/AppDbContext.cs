using Microsoft.EntityFrameworkCore;
using Telemedicina.Domain.Entities;

namespace Telemedicina.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Doctor> Doctors { get; set; } = null!;
    public DbSet<Patient> Patients { get; set; } = null!;
    public DbSet<Appointment> Appointments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Additional configurations (Fluent API) if necessary.
        modelBuilder.Entity<Doctor>().HasIndex(d => d.Cpf).IsUnique();
        modelBuilder.Entity<Patient>().HasIndex(p => p.Cpf).IsUnique();

        modelBuilder.Entity<Patient>()
            .HasOne(p => p.Doctor)
            .WithMany()
            .HasForeignKey(p => p.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
