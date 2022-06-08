using ClinicaVistaalegre.Server.Models;
using ClinicaVistaalegre.Shared.Models;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ClinicaVistaalegre.Server.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {


        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cita>()
            .HasOne(p => p.Paciente)
            .WithMany(c => c.Citas)
            .HasForeignKey(p => p.PacienteId);

            modelBuilder.Entity<Cita>()
            .HasOne(p => p.Medico)
            .WithMany(c => c.Citas)
            .HasForeignKey(p => p.MedicoId);


            modelBuilder.Entity<Mensaje>()
            .HasOne(p => p.Paciente)
            .WithMany(c => c.Mensajes)
            .HasForeignKey(p => p.PacienteId);

            modelBuilder.Entity<Mensaje>()
            .HasOne(p => p.Medico)
            .WithMany(c => c.Mensajes)
            .HasForeignKey(p => p.MedicoId);

            modelBuilder.Entity<Mensaje>()
                .HasKey(c => new
                {
                    c.Id
                });

            modelBuilder.Entity<ApplicationUser>().Property(e => e.Apellidos);
            modelBuilder.Ignore<Conversacion>();
        }

        public DbSet<Cita> Citas { get; set; }
        public DbSet<Mensaje> Mensajes { get; set; }
        public DbSet<Medico> Medicos { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
    }
}