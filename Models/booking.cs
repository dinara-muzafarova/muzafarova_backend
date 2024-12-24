using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace muzafarova_backend.Models
{
    public class booking
    {
        [Key]
        public int Id { get; set; } // Уникальный идентификатор бронирования

        [Required]
        [ForeignKey("Flight")]
        public int FlightId { get; set; } // ID связанного рейса

        [Required]
        [ForeignKey("Passenger")]
        public int PassengerId { get; set; } // ID связанного пассажира

        [Required]
        public DateTime BookingDate { get; set; } = DateTime.Now;
        public int SeatsBooked { get; set; } // Количество забронированных мест

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Price must be a positive value")]

        public int TotalPrice { get; set; } // Общая стоимость бронирования

        // Навигационные свойства (опционально, для EF Core, но скрыты из ввода)
        public virtual flight Flight { get; set; } // Связанный рейс
        public virtual passenger Passenger { get; set; } // Связанный пассажир
    }
}
