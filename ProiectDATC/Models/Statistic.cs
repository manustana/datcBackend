namespace ProiectDATC.Models
{
    public class Statistic
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Resolved { get; set; }
        public int Pending { get; set; }
    }
}