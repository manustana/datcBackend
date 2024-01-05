namespace ProiectDATC.Models
{
    public class ReportDto
    {
        public int? UserId { get; set; }

        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string? Status { get; set; }
    }
}