namespace TaskFlow.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        // Хранение хэша пароля
        public string PasswordHash { get; set; }
    }
}
