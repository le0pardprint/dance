namespace dance.API.Models.Dto
{
    public class RegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }  // Admin, Teacher, Accountant, Director, Client
        public int? Client_id { get; set; }  // если регистрируется клиент
        public int? Trainer_id { get; set; } // если регистрируется тренер
    }
}