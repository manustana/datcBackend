using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace ProiectDATC.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }

        public ICollection<Report> Reports = new Collection<Report>();
    }
}
