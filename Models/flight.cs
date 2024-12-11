namespace muzafarova_backend.Models
{
    public class flight
    {
        public int Id { get; set; }
        public string DepartureCity { get; set; }
        public string ArrivalCity { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }

        // Список бронирований на этот рейс
        //public List<Booking> Bookings { get; set; }

        // Метод для обновления количества доступных мест при создании нового бронирования
        //public void UpdateAvailableSeats()
        //{
        //    AvailableSeats = TotalSeats - Bookings.Count;
        //}
    }
}
