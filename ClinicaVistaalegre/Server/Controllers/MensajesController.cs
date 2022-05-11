using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicaVistaalegre.Server.Data;
using ClinicaVistaalegre.Shared.Models;

namespace ClinicaVistaalegre.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MensajesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient Http;

        public MensajesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Mensajes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Mensaje>>> GetMensajes()
        {
          if (_context.Mensajes == null)
          {
              return NotFound();
          }
            return await _context.Mensajes.ToListAsync();
        }

        // GET: api/Mensajes/Paciente/{pacienteId}/Medico/{medicoId}
        [Route("Paciente/{pacienteId}/Medico/{medicoId}")]
        public async Task<ActionResult<IEnumerable<Mensaje>>> GetMensajesPacienteMedico(string pacienteId, string medicoId)
        {
            if (_context.Mensajes == null)
            {
                return NotFound();
            }
            var mensajes = _context.Mensajes.Where(x => x.PacienteId == pacienteId).Where(x => x.MedicoId == medicoId);
            return await _context.Mensajes.ToListAsync();
        }

        [Route("ConversacionesByUser/{userId}")]
        public async Task<ActionResult<IEnumerable<Conversacion>>> GetConversaciones(string userId)
        {
            List<Conversacion> conversaciones = new List<Conversacion>();
            List<Mensaje> mensajes = new List<Mensaje>();
            if(_context.Medicos.Where(x => x.Id == userId).ToList().Any())
            {
                mensajes = _context.Mensajes.Where(x => x.PacienteId == userId).ToList();
            }
            else
            {
                mensajes = _context.Mensajes.Where(x => x.MedicoId == userId).ToList();
            }
            

            return conversaciones;
        }

            [Route("MedicosByPaciente/{pacienteId}")]
        public async Task<ActionResult<IEnumerable<Medico>>> GetMedicos(string pacienteId)
        {
            List<Medico> medicos = new List<Medico>();
            if (_context.Mensajes == null)
            {
                return NotFound();
            }
            var mensajes = _context.Mensajes.Where(x => x.PacienteId == pacienteId);
            foreach (var mensaje in mensajes)
            {
                medicos.Add(await Http.GetFromJsonAsync<Medico>($"api/Medicos/{mensaje.MedicoId}"));
            }

            return medicos;
        }

        [Route("PacientesByMedico/{medicoId}")]
        public async Task<ActionResult<IEnumerable<Paciente>>> GetPacientes(string medicoId)
        {
            List<Paciente> pacientes = new List<Paciente>();
            if (_context.Mensajes == null)
            {
                return NotFound();
            }
            var mensajes = _context.Mensajes.Where(x => x.MedicoId == medicoId);
            foreach (var mensaje in mensajes)
            {
                pacientes.Add(await Http.GetFromJsonAsync<Paciente>($"api/Medicos/{mensaje.MedicoId}"));
            }

            return pacientes;
        }

        // GET: api/Mensajes/5
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

        // PUT: api/Mensajes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMensaje(string id, Mensaje mensaje)
        {
            if (id != mensaje.PacienteId)
            {
                return BadRequest();
            }

            _context.Entry(mensaje).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MensajeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Mensajes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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

        // DELETE: api/Mensajes/5
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
