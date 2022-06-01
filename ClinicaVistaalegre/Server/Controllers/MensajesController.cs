using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicaVistaalegre.Server.Data;
using ClinicaVistaalegre.Shared.Models;
using Microsoft.AspNetCore.Authorization;

namespace ClinicaVistaalegre.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MensajesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient Http;

        public MensajesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //devuelve todos los mensajes del sistema
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Mensaje>>> GetMensajes()
        {
          if (_context.Mensajes == null)
          {
              return NotFound();
          }
            return await _context.Mensajes.ToListAsync();
        }

        //devuelve una conversación (mensajes entre un paciente y un médico)
        [HttpGet]
        [Route("Paciente/{pacienteId}/Medico/{medicoId}")]
        public async Task<ActionResult<IEnumerable<Mensaje>>> GetMensajesPacienteMedico(string pacienteId, string medicoId)
        {
            if (_context.Mensajes == null)
            {
                return NotFound();
            }
            var mensajes = _context.Mensajes.Where(x => x.PacienteId == pacienteId && x.MedicoId == medicoId);
            return await mensajes.ToListAsync();
        }

        //borra una conversación (mensajes entre un paciente y un médico)
        [HttpDelete("Paciente/{pacienteId}/Medico/{medicoId}")]
        public async Task<IActionResult> DeleteMensajesPacienteMedico(string pacienteId, string medicoId)
        {
            if (_context.Mensajes != null)
            {
                var mensajes = _context.Mensajes.Where(x => x.PacienteId == pacienteId && x.MedicoId == medicoId);
                foreach(var mensaje in mensajes)
                {
                    _context.Mensajes.Remove(mensaje);
                }
                await _context.SaveChangesAsync();
            }
            return NoContent();
        }

        //devuelve las conversaciones (mensajes entre un paciente y un médico) de un usuario
        [HttpGet]
        [Route("ConversacionesByUser/{userId}")]
        public async Task<ActionResult<IEnumerable<Conversacion>>> GetConversaciones(string userId)
        {
            List<Conversacion> conversaciones = new List<Conversacion>();
            List<Mensaje> mensajes = new List<Mensaje>();
            if(!_context.Medicos.Where(x => x.Id == userId).ToList().Any())
            {
                mensajes = _context.Mensajes.Where(x => x.PacienteId == userId).ToList();
                foreach (var mensaje in mensajes)
                {
                    var conversacion = new Conversacion()
                    {
                        destinatarioId = mensaje.MedicoId,
                        ContenidoPrimerMensaje = mensaje.Contenido,
                        FechaUltimoMensaje = mensaje.FechaHora
                    };
                    if (!conversaciones.Contains(conversacion))
                    {
                        conversaciones.Add(conversacion);
                    }
                }
            }
            else
            {
                mensajes = _context.Mensajes.Where(x => x.MedicoId == userId).ToList();
                foreach (var mensaje in mensajes)
                {
                    var conversacion = new Conversacion()
                    {
                        destinatarioId = mensaje.PacienteId,
                            ContenidoPrimerMensaje = mensaje.Contenido,
                            FechaUltimoMensaje = mensaje.FechaHora
                    };
                    if (!conversaciones.Contains(conversacion))
                    {
                        conversaciones.Add(conversacion);
                    }
                }
            }

            return conversaciones;
        }

        //devuelve un mensaje por su id
        [HttpGet("{id}")]
        public async Task<ActionResult<Mensaje>> GetMensaje(string id)
        {
          if (_context.Mensajes == null)
          {
              return NotFound();
          }
            var mensaje = await _context.Mensajes.FindAsync(id);

            if (mensaje == null)
            {
                return NotFound();
            }

            return mensaje;
        }

        //inserta un mensaje
        [HttpPost]
        public async Task<ActionResult<Mensaje>> PostMensaje(Mensaje mensaje)
        {
          if (_context.Mensajes == null)
          {
              return Problem("Entity set 'ApplicationDbContext.Mensajes'  is null.");
          }
            _context.Mensajes.Add(mensaje);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MensajeExists(mensaje.PacienteId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetMensaje", new { id = mensaje.PacienteId }, mensaje);
        }

        //borra un mensaje
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMensaje(string id)
        {
            if (_context.Mensajes == null)
            {
                return NotFound();
            }
            var mensaje = await _context.Mensajes.FindAsync(id);
            if (mensaje == null)
            {
                return NotFound();
            }

            _context.Mensajes.Remove(mensaje);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MensajeExists(string id)
        {
            return (_context.Mensajes?.Any(e => e.PacienteId == id)).GetValueOrDefault();
        }
    }
}
