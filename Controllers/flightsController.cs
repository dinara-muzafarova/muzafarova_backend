using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using muzafarova_backend.Data;
using muzafarova_backend.Models;

namespace muzafarova_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class flightsController : ControllerBase
    {
        private readonly muzafarova_backendContext _context;

        public flightsController(muzafarova_backendContext context)
        {
            _context = context;
        }

        // GET: api/flights
        [HttpGet]
        public async Task<ActionResult<IEnumerable<flight>>> Getflight()
        {
            return await _context.flight.ToListAsync();
        }

        // GET: api/flights/5
        [HttpGet("{id}")]
        public async Task<ActionResult<flight>> Getflight(int id)
        {
            var flight = await _context.flight.FindAsync(id);

            if (flight == null)
            {
                return NotFound();
            }

            return flight;
        }

        // PUT: api/flights/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Putflight(int id, flight flight)
        {
            if (id != flight.Id)
            {
                return BadRequest();
            }

            _context.Entry(flight).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!flightExists(id))
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

        // POST: api/flights
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<flight>> Postflight(flight flight)
        {
            _context.flight.Add(flight);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Getflight", new { id = flight.Id }, flight);
        }

        // DELETE: api/flights/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deleteflight(int id)
        {
            var flight = await _context.flight.FindAsync(id);
            if (flight == null)
            {
                return NotFound();
            }

            _context.flight.Remove(flight);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool flightExists(int id)
        {
            return _context.flight.Any(e => e.Id == id);
        }
    }
}
