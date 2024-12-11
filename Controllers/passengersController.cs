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
    public class passengersController : ControllerBase
    {
        private readonly muzafarova_backendContext _context;

        public passengersController(muzafarova_backendContext context)
        {
            _context = context;
        }

        // GET: api/passengers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<passenger>>> Getpassenger()
        {
            return await _context.passenger.ToListAsync();
        }

        // GET: api/passengers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<passenger>> Getpassenger(int id)
        {
            var passenger = await _context.passenger.FindAsync(id);

            if (passenger == null)
            {
                return NotFound();
            }

            return passenger;
        }

        // PUT: api/passengers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Putpassenger(int id, passenger passenger)
        {
            if (id != passenger.Id)
            {
                return BadRequest();
            }

            _context.Entry(passenger).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!passengerExists(id))
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

        // POST: api/passengers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<passenger>> Postpassenger(passenger passenger)
        {
            _context.passenger.Add(passenger);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Getpassenger", new { id = passenger.Id }, passenger);
        }

        // DELETE: api/passengers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletepassenger(int id)
        {
            var passenger = await _context.passenger.FindAsync(id);
            if (passenger == null)
            {
                return NotFound();
            }

            _context.passenger.Remove(passenger);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool passengerExists(int id)
        {
            return _context.passenger.Any(e => e.Id == id);
        }
    }
}
