namespace ClinicaVistaalegre.Shared.Models
{
    public class Conversacion
    {
        public string destinatarioId { get; set; }
        public string? Apellidos { get; set; }
        public string ContenidoPrimerMensaje { get; set; }
        public DateTime FechaUltimoMensaje { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Conversacion conversacion &&
                   destinatarioId == conversacion.destinatarioId;
        }
    }
}
