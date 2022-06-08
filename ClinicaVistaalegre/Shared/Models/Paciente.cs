using Newtonsoft.Json;

namespace ClinicaVistaalegre.Shared.Models
{
    public class Paciente
    {
        public string Id { get; set; }
        public string Apellidos { get; set; }
        public DateTime FechaDeNacimiento { get; set; }
        public char Sexo { get; set; }
        [JsonProperty(Required = Required.AllowNull)]
        public List<Cita>? Citas { get; set; }
        [JsonProperty(Required = Required.AllowNull)]
        public List<Mensaje>? Mensajes { get; set; }
    }
}
