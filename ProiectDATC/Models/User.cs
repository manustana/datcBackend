using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace ProiectDATC.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required(ErrorMessage = "CNP is required")]
        [MaxLength(13, ErrorMessage = "CNP must be exactly 13 characters")]
        [MinLength(13, ErrorMessage = "CNP must be exactly 13 characters")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "CNP must consist of only digits")]
        
        public string Cnp { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
        public int? Points { get; set; } = 0;

        public ICollection<Report> Reports = new Collection<Report>();
    }
}
