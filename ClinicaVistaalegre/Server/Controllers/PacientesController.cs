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
    public class PacientesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PacientesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //devuelve todos los pacientes del sistema
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Paciente>>> GetPacientes()
        {
          if (_context.Pacientes == null)
          {
              return NotFound();
          }
            return await _context.Pacientes.ToListAsync();
        }

        //devuelve un paciente según id
        [HttpGet("{id}")]
        public async Task<ActionResult<Paciente>> GetPaciente(string id)
        {
          if (_context.Pacientes == null)
          {
              return NotFound();
          }
            var paciente = await _context.Pacientes.FindAsync(id);

            if (paciente == null)
            {
                return NotFound();
            }

            return paciente;
        }

        //actualiza un paciente
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPaciente(string id, Paciente paciente)
        {
            if (id != paciente.Id)
            {
                return BadRequest();
            }

            _context.Entry(paciente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PacienteExists(id))
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

        //inserta un paciente
        [HttpPost]
        public async Task<ActionResult<Paciente>> PostPaciente(Paciente paciente)
        {
          if (_context.Pacientes == null)
          {
              return Problem("Entity set 'ApplicationDbContext.Pacientes'  is null.");
          }
            _context.Pacientes.Add(paciente);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PacienteExists(paciente.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPaciente", new { id = paciente.Id }, paciente);
        }

        //borra un paciente
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaciente(string id)
        {
            if (_context.Pacientes == null)
            {
                return NotFound();
            }
            var paciente = await _context.Pacientes.FindAsync(id);
            if (paciente == null)
            {
                return NotFound();
            }

            _context.Pacientes.Remove(paciente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PacienteExists(string id)
        {
            return (_context.Pacientes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
