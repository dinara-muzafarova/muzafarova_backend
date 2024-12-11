namespace muzafarova_backend.Models
{
    public class passenger
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }

        // Список бронирований пассажира
        //public List<Booking> Bookings { get; set; }

        // Метод для добавления нового бронирования пассажиру
        //public void AddBooking(Booking booking)
        //{
        //    if (Bookings == null)
        //        Bookings = new List<Booking>();

        //    Bookings.Add(booking);
        //}
    }
}
