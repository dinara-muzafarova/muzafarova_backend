using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
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
            return await _context.Passenger.ToListAsync();
        }

        // GET: api/passengers/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Getpassenger(int id)
        {
            // Проверяем, существует ли пассажир
            var passenger = await _context.Passenger
                .Include(p => p.Bookings)
                .ThenInclude(b => b.Flight)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (passenger == null)
            {
                return NotFound("Пассажир не найден.");
            }

            // Формируем объект для ответа
            var response = new
            {
                Passenger = new PassengerDTO
                {
                    Id = passenger.Id,
                    FirstName = passenger.FirstName,
                    LastName = passenger.LastName,
                    Email = passenger.Email,
                    Phone = passenger.Phone,
                    BirthDate = passenger.BirthDate
                },
                Flights = passenger.Bookings
                    .Select(b => new FlightDTO
                    {
                        Id = b.Flight.Id,
                        FlightNumber = b.Flight.FlightNumber,
                        DepartureCity = b.Flight.DepartureCity,
                        ArrivalCity = b.Flight.ArrivalCity,
                        DepartureTime = b.Flight.DepartureTime,
                        ArrivalTime = b.Flight.ArrivalTime,
                        TotalSeats = b.Flight.TotalSeats,
                        AvailableSeats = b.Flight.AvailableSeats,
                        PricePerSeat = b.Flight.PricePerSeat
                    })
                    .ToList()
            };

            return Ok(response);
        }

        // PUT: api/passengers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Putpassenger(int id, [FromBody] PassengerDTO passengerDto)
        {
            // Проверяем, существует ли пассажир с указанным ID
            var existingPassenger = await _context.Passenger.FindAsync(id);
            if (existingPassenger == null)
            {
                return NotFound("Пассажир не найден.");
            }

            // Обновляем данные пассажира только на основе PassengerDTO
            existingPassenger.FirstName = passengerDto.FirstName;
            existingPassenger.LastName = passengerDto.LastName;
            existingPassenger.Email = passengerDto.Email;
            existingPassenger.Phone = passengerDto.Phone;
            existingPassenger.BirthDate = passengerDto.BirthDate;

            try
            {
                // Сохраняем изменения в базе данных
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

            return Ok("Данные пассажира успешно обновлены.");
        }




        // POST: api/passengers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PassengerDTO>> Postpassenger(PassengerDTO dto)
        {

            // Проверяем, существует ли уже пассажир с таким Email
            var existingPassenger = await _context.Passenger
                .FirstOrDefaultAsync(p => p.Email == dto.Email);

            if (existingPassenger != null)
            {
                return BadRequest(new
                {
                    message = "A passenger with this email already exists."
                });
            }
            // Создаем нового пассажира из входящего DTO
            var passenger = new passenger
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                BirthDate = dto.BirthDate,
                Bookings = new List<booking>() // Пустой список бронирований
            };

          


            _context.Passenger.Add(passenger);
            await _context.SaveChangesAsync();

            // Преобразуем в PassengerDTO перед возвратом
            var passengerDto = new PassengerDTO
            {
                Id = passenger.Id,
                FirstName = passenger.FirstName,
                LastName = passenger.LastName,
                Email = passenger.Email,
                Phone = passenger.Phone,
                BirthDate = passenger.BirthDate
            };

            return Ok(passengerDto);
        }

        // DELETE: api/passengers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletepassenger(int id)
        {
            var passenger = await _context.Passenger.FindAsync(id);
            if (passenger == null)
            {
                return NotFound();
            }

            _context.Passenger.Remove(passenger);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool passengerExists(int id)
        {
            return _context.Passenger.Any(e => e.Id == id);
        }
    }
}
