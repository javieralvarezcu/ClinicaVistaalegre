using Microsoft.AspNetCore.Identity;

namespace ClinicaVistaalegre.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Apellidos { get; set; }
        public DateTime? FechaDeNacimiento { get; set; }
        public char? Sexo { get; set; }
        public string? Especialidad { get; set; }


    }
}