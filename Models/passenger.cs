using Newtonsoft.Json;

namespace muzafarova_backend.Models
{
    public class passenger
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int Phone { get; set; }
        public DateTime BirthDate { get; set; }

        // Список бронирований пассажира

        public List<booking> Bookings { get; set; }

        // Добавление бронирования
        /*public void AddBooking(booking booking)
        {
            Bookings.Add(booking);
        }*/
    }
}
