using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicaVistaalegre.Shared.Models
{
    public class Medico
    {
        public string Id { get; set; }
        public string Apellidos { get; set; }
        public string Especialidad { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public List<Cita> Citas { get; set; }
        [JsonProperty(Required = Required.AllowNull)]
        public List<Mensaje> Mensajes { get; set; }
    }
}
