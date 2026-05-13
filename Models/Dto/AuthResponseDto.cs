namespace dance.API.Models.Dto
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public int UserId { get; set; }
    }
}