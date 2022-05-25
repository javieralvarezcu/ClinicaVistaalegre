using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ClinicaVistaalegre.Shared.Models
{
    public class Mensaje
    {
        [Key]
        public int Id { get; set; }
        public string PacienteId { get; set; }
        [JsonIgnore]
        [JsonProperty(Required = Required.AllowNull)]
        public Paciente? Paciente { get; set; }
        public string MedicoId { get; set; }
        [JsonIgnore]
        [JsonProperty(Required = Required.AllowNull)]
        public Medico? Medico { get; set; }

        public string Contenido { get; set; }
        public DateTime FechaHora { get; set; }

        public string Emisor { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Mensaje mensaje &&
                   PacienteId == mensaje.PacienteId &&
                   MedicoId == mensaje.MedicoId &&
                   Contenido == mensaje.Contenido &&
                   FechaHora == mensaje.FechaHora;
        }
    }
}
