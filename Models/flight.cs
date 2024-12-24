using Newtonsoft.Json;

namespace muzafarova_backend.Models
{
    public class flight
    {
        public int Id { get; set; }

        public string FlightNumber { get; set; }
        public string DepartureCity { get; set; } //город отправления
        public string ArrivalCity { get; set; } //город прибытия
        public DateTime DepartureTime { get; set; } //время отправления
        public DateTime ArrivalTime { get; set; } //время прилета
        public int TotalSeats { get; set; } //общее количество мест
        public int AvailableSeats { get; set; } //доступное колво мест

        public int PricePerSeat { get; set; } // Цена за место


        // Список бронирований на этот рейс
        [JsonIgnore]
        public List<booking> Bookings { get; set; }


        public bool HasAvailableSeats()
        {
            return AvailableSeats > 0;
        }
    }
}
