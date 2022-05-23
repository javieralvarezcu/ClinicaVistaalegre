﻿#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicaVistaalegre.Server.Data;
using ClinicaVistaalegre.Shared.Models;
using Microsoft.AspNetCore.Authorization;

namespace ClinicaVistaalegre.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CitasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CitasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Citas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cita>>> GetCitas()
        {
            var response = await _context.Citas.ToListAsync();
            foreach (var cita in response)
            {
                if (cita.FechaHora < DateTime.Now)
                {
                    cita.Estado = "Pasada";
                }
            }
            return response;
        }

        // GET: api/CitasByPaciente/asdfg-qwerty-zxcvbn
        [Route("ByPaciente/{id}")]
        public async Task<ActionResult<List<Cita>>> GetCitasByPaciente(string id)
        {
            var response = await _context.Citas.ToListAsync();
            foreach (var cita in response)
            {
                if (cita.FechaHora < DateTime.Now)
                {
                    cita.Estado = "Pasada";
                }
            }
            return response.Where(x => x.PacienteId==id).ToList();
        }

        // GET: api/CitasByMedico/asdfg-qwerty-zxcvbn
        [Route("ByMedico/{id}")]
        public async Task<ActionResult<List<Cita>>> GetCitasByMedico(string id)
        {
            var response = await _context.Citas.ToListAsync();
            foreach(var cita in response)
            {
                if (cita.FechaHora < DateTime.Now)
                {
                    cita.Estado = "Pasada";
                }
            }
            return response.Where(x => x.MedicoId == id).OrderBy(x => x.FechaHora).ToList();
        }

        [Route("HorasByMedico/{medicoId}/{pacienteId}/{date}")]
        public async Task<ActionResult<List<DateTime>>> GetHorasByMedico(string medicoId, string pacienteId, string date)
        {
            var response = _context.Citas.ToListAsync().Result;
            List<Cita> citasDiaMedico = response.Where(d => d.FechaHora.ToString("dd-MM-yyyy") == date && d.MedicoId==medicoId)
                .ToList();
            var horas = citasDiaMedico
                .Where(d => d.Estado!="Cancelada" && d.PacienteId==pacienteId)
                .Select(d => d.FechaHora).ToList();
            List<DateTime> horasDelDia = new List<DateTime>();
            DateTime tempHora = new DateTime().AddHours(8);
            do
            {
                horasDelDia.Add(tempHora);
                tempHora=tempHora.AddMinutes(10);
            } while (tempHora.Hour < 17);

            foreach(var hora in horas)
            {
                horasDelDia.Remove(new DateTime().AddHours(hora.Hour).AddMinutes(hora.Minute));
            }

            return horasDelDia;
        }

        // GET: api/Citas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cita>> GetCita(int id)
        {
            var cita = await _context.Citas.FindAsync(id);

            if (cita == null)
            {
                return NotFound();
            }

            return cita;
        }

        // PUT: api/Citas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCita(int id, Cita cita)
        {
            if (id != cita.Id)
            {
                return BadRequest();
            }

            _context.Entry(cita).State = EntityState.Modified;

            try
            {
                var mensaje = new Mensaje()
                {
                    Contenido = "",
                    Emisor = "",
                    MedicoId = cita.MedicoId,
                    PacienteId = cita.PacienteId,
                    FechaHora = DateTime.Now
                };

                switch (cita.Estado)
                {
                    case "Aceptada":
                        mensaje.Contenido = $"*Aceptada cita con el motivo: {cita.Motivo} y fecha: {cita.FechaHora.ToString("dd-MM-yyyy HH:mm")}*";
                        mensaje.Emisor = cita.MedicoId;
                        break;
                    case "Cancelada":
                        mensaje.Contenido = $"*Cancelada cita con el motivo: {cita.Motivo} y fecha: {cita.FechaHora.ToString("dd-MM-yyyy HH:mm")}*";
                        mensaje.Emisor = cita.MedicoId;
                        break;
                }
                if (cita.Estado.Equals(""))
                {
                    mensaje.Contenido = $"*Modificada cita con el motivo: {cita.Motivo} y fecha: {cita.FechaHora.ToString("dd-MM-yyyy HH:mm")}*";
                    mensaje.Emisor = cita.PacienteId;
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CitaExists(id))
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

        // POST: api/Citas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Cita>> PostCita(Cita cita)
        {
            _context.Citas.Add(cita);
            _context.Mensajes.Add(new Mensaje()
            {
                Contenido = $"*Solicitada cita con el motivo: {cita.Motivo} y fecha: {cita.FechaHora.ToString("dd-MM-yyyy HH:mm")}*",
                Emisor = cita.PacienteId,
                MedicoId = cita.MedicoId,
                PacienteId = cita.PacienteId,
                FechaHora = DateTime.Now
            });
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCita", new { id = cita.Id }, cita);
        }

        // DELETE: api/Citas/5
        [HttpDelete("{id}/{emisor}")]
        public async Task<IActionResult> DeleteCita(int id, string emisor)
        {
            var cita = await _context.Citas.FindAsync(id);
            if (cita == null)
            {
                return NotFound();
            }

            _context.Citas.Remove(cita);
            _context.Mensajes.Add(new Mensaje()
            {
                Contenido = $"*Eliminada cita con el motivo: {cita.Motivo} y fecha: {cita.FechaHora.ToString("dd-MM-yyyy HH:mm")}*",
                Emisor = emisor,
                MedicoId = cita.MedicoId,
                PacienteId = cita.PacienteId,
                FechaHora = DateTime.Now
            });
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CitaExists(int id)
        {
            return _context.Citas.Any(e => e.Id == id);
        }
    }
}
