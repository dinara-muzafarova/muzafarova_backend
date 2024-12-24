using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<ActionResult<IEnumerable<FlightDTO>>> Getflight()
        {
            // Извлекаем рейсы из базы данных
            var flights = await _context.Flight.ToListAsync();

            // Преобразуем в DTO
            var flightDtos = flights.Select(f => new FlightDTO
            {
                Id = f.Id,
                FlightNumber = f.FlightNumber,
                DepartureCity = f.DepartureCity,
                ArrivalCity = f.ArrivalCity,
                DepartureTime = f.DepartureTime,
                ArrivalTime = f.ArrivalTime,
                TotalSeats = f.TotalSeats,
                AvailableSeats = f.AvailableSeats,
                PricePerSeat = f.PricePerSeat
            }).ToList();

            return Ok(flightDtos); // Возвращаем список DTO
        }



        // GET: api/flights/5
        [HttpGet("{id}")]
        
        public async Task<ActionResult<flight>> Getflight(int id)
        {
            var flight = await _context.Flight.FindAsync(id);

            if (flight == null)
            {
                return NotFound();
            }

            return flight;
        }

        // PUT: api/flights/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Putflight(int id, [FromBody] FlightDTO flightDto)
        {
            // Проверяем, существует ли рейс с указанным ID
            var existingFlight = await _context.Flight.FindAsync(id);
            if (existingFlight == null)
            {
                return NotFound("Рейс с указанным ID не найден.");
            }

            // Обновляем данные рейса
            existingFlight.DepartureCity = flightDto.DepartureCity;
            existingFlight.ArrivalCity = flightDto.ArrivalCity;
            existingFlight.DepartureTime = flightDto.DepartureTime;
            existingFlight.ArrivalTime = flightDto.ArrivalTime;
            existingFlight.AvailableSeats = flightDto.AvailableSeats;
            existingFlight.PricePerSeat = flightDto.PricePerSeat;

            // Сохраняем изменения в базе данных
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest("Ошибка при обновлении данных рейса: " + ex.Message);
            }

            return Ok("Рейс успешно обновлен.");
        }


        // POST: api/flights
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<FlightDTO>> Postflight(FlightDTO dto)
        {

            // Создаем нового пассажира из входящего DTO
            var flight = new flight
            {
                FlightNumber = dto.FlightNumber,
                DepartureCity = dto.DepartureCity,
                ArrivalCity = dto.ArrivalCity,
                DepartureTime = dto.DepartureTime,
                ArrivalTime = dto.ArrivalTime,
                TotalSeats = dto.TotalSeats,
                AvailableSeats = dto.AvailableSeats,
                PricePerSeat = dto.PricePerSeat,
                Bookings = new List<booking>() // Пустой список бронирований
            };




            _context.Flight.Add(flight);
            await _context.SaveChangesAsync();

            // Преобразуем в PassengerDTO перед возвратом
            var flightDto = new FlightDTO
            {
                FlightNumber = flight.FlightNumber,
                DepartureCity = flight.DepartureCity,
                ArrivalCity = flight.ArrivalCity,
                DepartureTime = flight.DepartureTime,
                ArrivalTime = flight.ArrivalTime,
                TotalSeats = flight.TotalSeats,
                AvailableSeats = flight.AvailableSeats,
                PricePerSeat = flight.PricePerSeat,
            };

            return Ok(flightDto);
        }


        [HttpGet("search")]
        public async Task<ActionResult> Searchflights(
            string departureCity,
            string arrivalCity,
            DateTime? departureDate = null, 
            bool includeConnections = false,
            TimeSpan minLayover = default(TimeSpan),
            TimeSpan maxLayover = default(TimeSpan))
        {
            // Установка минимального и максимального времени пересадки (если они не указаны)
            if (minLayover == default) minLayover = TimeSpan.FromHours(1);
            if (maxLayover == default) maxLayover = TimeSpan.FromHours(6);

            // 1. Поиск прямых рейсов
            var directFlightsQuery = _context.Flight
                .Where(f => f.DepartureCity == departureCity && f.ArrivalCity == arrivalCity);

            // Применяем фильтр по дате, если она указана
            if (departureDate.HasValue)
            {
                directFlightsQuery = directFlightsQuery.Where(f => f.DepartureTime.Date == departureDate.Value.Date);
            }

            var directFlights = await directFlightsQuery.ToListAsync();

            // Формируем результат для прямых рейсов
            var results = new List<object>();
            foreach (var flight in directFlights)
            {
                results.Add(new
                {
                    Type = "Direct",
                    Flight = new
                    {
                        flight.Id,
                        flight.FlightNumber,
                        flight.DepartureCity,
                        flight.ArrivalCity,
                        flight.DepartureTime,
                        flight.ArrivalTime,
                        flight.TotalSeats,
                        flight.AvailableSeats,
                        flight.PricePerSeat
                    }
                });
            }

            // 2. Если включен поиск с пересадками
            if (includeConnections)
            {
                // Получаем все рейсы, вылетающие из города отправления
                var firstLegFlightsQuery = _context.Flight
                    .Where(f => f.DepartureCity == departureCity);

                // Применяем фильтр по дате, если она указана
                if (departureDate.HasValue)
                {
                    firstLegFlightsQuery = firstLegFlightsQuery.Where(f => f.DepartureTime.Date == departureDate.Value.Date);
                }

                var firstLegFlights = await firstLegFlightsQuery.ToListAsync();

                foreach (var flight1 in firstLegFlights)
                {
                    // Находим рейсы, вылетающие из пункта назначения первого рейса
                    var connectingFlights = await _context.Flight
                        .Where(f => f.DepartureCity == flight1.ArrivalCity &&
                                    f.DepartureTime > flight1.ArrivalTime.Add(minLayover) &&
                                    f.DepartureTime <= flight1.ArrivalTime.Add(maxLayover) &&
                                    f.ArrivalCity == arrivalCity)
                        .ToListAsync();

                    // Добавляем комбинации рейсов в результат
                    foreach (var flight2 in connectingFlights)
                    {
                        results.Add(new
                        {
                            Type = "Connecting",
                            FirstLeg = new
                            {
                                flight1.Id,
                                flight1.FlightNumber,
                                flight1.DepartureCity,
                                flight1.ArrivalCity,
                                flight1.DepartureTime,
                                flight1.ArrivalTime,
                                flight1.TotalSeats,
                                flight1.AvailableSeats,
                                flight1.PricePerSeat
                            },
                            SecondLeg = new
                            {
                                flight2.Id,
                                flight2.FlightNumber,
                                flight2.DepartureCity,
                                flight2.ArrivalCity,
                                flight2.DepartureTime,
                                flight2.ArrivalTime,
                                flight2.TotalSeats,
                                flight2.AvailableSeats,
                                flight2.PricePerSeat
                            },
                            TotalPrice = flight1.PricePerSeat + flight2.PricePerSeat
                        });
                    }
                }
            }

            // 3. Если ничего не найдено
            if (results.Count == 0)
            {
                return NotFound("Рейсы не найдены.");
            }

            // 4. Возвращаем результаты
            return Ok(results);
        }



        // DELETE: api/flights/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Deleteflight(int id)
        {
            var flight = await _context.Flight.FindAsync(id);
            if (flight == null)
            {
                return NotFound();
            }

            _context.Flight.Remove(flight);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool flightExists(int id)
        {
            return _context.Flight.Any(e => e.Id == id);
        }
    }
}
