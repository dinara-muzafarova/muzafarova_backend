using Microsoft.Build.Framework;

namespace muzafarova_backend
{
    public class BookingCreateDto
    {
        public int FlightId { get; set; }
        public int PassengerId { get; set; }
        public int SeatsBooked { get; set; }
    }

}

