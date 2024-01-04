// Models/Report.cs

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProiectDATC.Models
{
    public class Report
    {
        public int Id { get; set; }

        // Make UserId nullable
        public int? UserId { get; set; }

        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }

    }
}
