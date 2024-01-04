using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using ProiectDATC.Models;
using System.Text.Json;
using System;
using System.Linq;
public class Functions
{
    private readonly AppDbContext _dbContext;

    public Functions(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [NoAutomaticTrigger]
    public void WeeklyStatisticsJob([TimerTrigger("0 0 0 * * SUN")] TimerInfo timerInfo, ILogger log)
    {
        // Calculate the date range for the last week (from the previous Sunday to today)
        DateTime lastSunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek - 7);
        DateTime today = DateTime.Today;

        // Retrieve reports added in the last week
        var reportsInLastWeek = _dbContext.Reports
            .Where(r => r.Date >= lastSunday && r.Date <= today)
            .ToList();

        // Perform statistics calculation (you can customize this based on your requirements)
        int totalReports = reportsInLastWeek.Count;
        int pendingReports = reportsInLastWeek.Count(r => r.Status == "PENDING");
        int resolvedReports = reportsInLastWeek.Count(r => r.Status == "DONE");

        // Log the statistics
        log.LogInformation($"Weekly Statistics (from {lastSunday.ToShortDateString()} to {today.ToShortDateString()}):");
        log.LogInformation($"Total Reports: {totalReports}");
        log.LogInformation($"Pending Reports: {pendingReports}");
        log.LogInformation($"Resolved Reports: {resolvedReports}");

    }
}
