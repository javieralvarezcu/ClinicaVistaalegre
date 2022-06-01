#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
    public class MedicosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MedicosController(ApplicationDbContext context)
        {
            _context = context;
        }

        //devuelve todos los médicos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Medico>>> GetMedicos()
        {
            return await _context.Medicos.ToListAsync();
        }

        //devuelve un médico según su id
        [HttpGet("{id}")]
        public async Task<ActionResult<Medico>> GetMedico(string id)
        {
            var medico = await _context.Medicos.FindAsync(id);

            if (medico == null)
            {
                return NotFound();
            }

            return medico;
        }

        //actualiza un médico
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMedico(string id, Medico medico)
        {
            if (id != medico.Id)
            {
                return BadRequest();
            }

            _context.Entry(medico).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedicoExists(id))
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

        //inserta un médico
        [HttpPost]
        public async Task<ActionResult<Medico>> PostMedico(Medico medico)
        {
            _context.Medicos.Add(medico);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MedicoExists(medico.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetMedico", new { id = medico.Id }, medico);
        }

        //borra un médico
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedico(string id)
        {
            var medico = await _context.Medicos.FindAsync(id);
            if (medico == null)
            {
                return NotFound();
            }

            _context.Medicos.Remove(medico);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MedicoExists(string id)
        {
            return _context.Medicos.Any(e => e.Id == id);
        }
    }
}
