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
    public class bookingsController : ControllerBase
    {
        private readonly muzafarova_backendContext _context;

        public bookingsController(muzafarova_backendContext context)
        {
            _context = context;
        }

        // GET: api/bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<booking>>> Getbooking()
        {
            var booking = await _context.Booking
                .Include(b => b.Passenger) // Подключаем данные пассажира
                .Include(b => b.Flight)    // Подключаем данные рейса
                .ToListAsync();

            if (booking == null || booking.Count == 0)
            {
                return NotFound("No bookings found.");
            }

            return Ok(booking);
        }

        // GET: api/bookings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<booking>> Getbooking(int id)
        {
            var booking = await _context.Booking
                .Include(b => b.Passenger) // Включаем информацию о пассажире
                .Include(b => b.Flight)    // Включаем информацию о рейсе
                .Where(b => b.Id == id)    // Фильтруем по ID
                .FirstOrDefaultAsync();

            if (booking == null)
            {
                return NotFound("Бронирование не найдено.");
            }

            return Ok(new
            {
                BookingId = booking.Id,
                SeatsBooked = booking.SeatsBooked,
                TotalPrice = booking.TotalPrice,
                BookingDate = booking.BookingDate,
                Passenger = new
                {
                    booking.Passenger.FirstName,
                    booking.Passenger.LastName,
                    booking.Passenger.Email,
                    booking.Passenger.Phone,
                    booking.Passenger.BirthDate
                },
                Flight = new
                {
                    booking.Flight.DepartureCity,
                    booking.Flight.ArrivalCity,
                    booking.Flight.DepartureTime,
                    booking.Flight.ArrivalTime,
                    booking.Flight.AvailableSeats,
                    booking.Flight.PricePerSeat
                }
            });
        }



        // PUT: api/bookings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Putbooking(int id, [FromBody] BookingCreateDto bookingDto)
        {
            var booking = await _context.Booking
                .Include(b => b.Flight)  // Включаем рейс для обновления информации о местах и цене
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound("Бронирование не найдено.");
            }

            // Проверка на доступность мест
            var availableSeats = booking.Flight.AvailableSeats + booking.SeatsBooked; // Количество мест, которое можно забронировать
            if (bookingDto.SeatsBooked > availableSeats)
            {
                return BadRequest("Недостаточно мест для изменения бронирования.");
            }

            // Обновляем количество мест и общую цену
            booking.SeatsBooked = bookingDto.SeatsBooked;
            booking.TotalPrice = booking.Flight.PricePerSeat * bookingDto.SeatsBooked;

            // Обновляем количество доступных мест
            booking.Flight.AvailableSeats = availableSeats - bookingDto.SeatsBooked;

            // Сохраняем изменения в базе данных
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!bookingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Бронирование успешно обновлено.");
        }

        // POST: api/bookings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostBooking(BookingCreateDto dto)
        {
            // Найти рейс по ID
            var flight = await _context.Flight.FindAsync(dto.FlightId);
            if (flight == null)
            {
                return NotFound("Рейс не найден");
            }

            // Проверка доступности мест
            if (flight.AvailableSeats < dto.SeatsBooked)
            {
                return BadRequest("Недостаточно свободных мест на рейс.");
            }

            // Рассчитываем цену на основе количества мест и цены за место
            int totalPrice = flight.PricePerSeat * dto.SeatsBooked;

            // Создаем новое бронирование
            var booking = new booking
            {
                FlightId = dto.FlightId,
                PassengerId = dto.PassengerId,
                SeatsBooked = dto.SeatsBooked,
                TotalPrice = totalPrice, // Автоматически рассчитанная цена
                BookingDate = DateTime.UtcNow
            };

            // Обновляем количество доступных мест на рейсе
            flight.AvailableSeats -= dto.SeatsBooked;

            // Добавляем бронирование и сохраняем изменения
            _context.Booking.Add(booking);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Бронирование успешно создано",
                BookingId = booking.Id,
                TotalPrice = booking.TotalPrice
            });
        }




        // DELETE: api/bookings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletebooking(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool bookingExists(int id)
        {
            return _context.Booking.Any(e => e.Id == id);
        }
    }
}
