using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicaVistaalegre.Shared.Models
{
    public class Cita
    {
        public int Id { get; set; }

        [Required]
        public string PacienteId { get; set; }
        [JsonProperty(Required = Required.AllowNull)]
        public Paciente? Paciente { get; set; }
        [Required]
        public string MedicoId { get; set; }
        [JsonProperty(Required = Required.AllowNull)]
        public Medico? Medico { get; set; }

        public string? Motivo { get; set; }
        [CitaDateValidation]
        public DateTime FechaHora { get; set; }
        public string Estado { get; set; } = "Pendiente";
    }
}
