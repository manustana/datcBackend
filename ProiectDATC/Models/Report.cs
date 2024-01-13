namespace ProiectDATC.Models
{
    public class Report
    {
        public int Id { get; set; }
        public int? UserId { get; set; }

        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }

    }
}
