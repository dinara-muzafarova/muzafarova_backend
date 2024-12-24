namespace muzafarova_backend
{
    public class PassengerDTO
    {
        public int Id { get; set; }         // ID пассажира
        public string FirstName { get; set; }  // Имя
        public string LastName { get; set; }   // Фамилия
        public string Email { get; set; }      // Электронная почта
        public int Phone { get; set; }         // Номер телефона
        public DateTime BirthDate { get; set; } // Дата рождения
    }
}
