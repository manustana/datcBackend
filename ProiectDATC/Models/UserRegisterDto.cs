namespace ProiectDATC.Models
{
    public class UserRegisterDto
    {
        public string Username { get; set; }
        public string Cnp { get; set; }
        public string Email { get; set; }
        public string? Role { get; set; }
        public string Password { get; set; }
    }
}