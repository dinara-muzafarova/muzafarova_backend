namespace muzafarova_backend.Models
{
    public class booking
    {
        public int Id { get; set; }
        public flight Flight { get; set; }
        public passenger Passenger { get; set; }
        public DateTime BookingDate { get; set; }
        public int Price { get; set; }

        // Метод для расчета стоимости билета
        //public void CalculatePrice()
        //{
        //    // Логика расчета цены может зависеть от различных факторов, таких как расстояние, класс обслуживания и т.д.
        //    // В данном примере просто фиксированная цена
        //    Price = 1000m;
        //}
    }
}
