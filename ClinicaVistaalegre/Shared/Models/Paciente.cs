using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicaVistaalegre.Shared.Models
{
    public class Paciente
    {
        public int Id { get; set; }
        public string Apellidos { get; set; }
        public DateTime FechaDeNacimiento { get; set; }
        public char Sexo { get; set; }
        [JsonProperty(Required = Required.AllowNull)]
        public List<Cita> Citas { get; set; }
        [JsonProperty(Required = Required.AllowNull)]
        public List<Mensaje> Mensajes { get; set; }
    }
}
