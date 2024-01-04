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

    public async Task processservicebus([ServiceBusTrigger("proiectdatc", Connection = "AzureWebJobsServiceBus")] string myQueueItem, ILogger log)
    {
        log.LogInformation($"Received message: {myQueueItem}");
        var reportDto = JsonSerializer.Deserialize<ReportDto>(myQueueItem);
        Console.WriteLine($"Received: {reportDto}");

        var report = new Report
        {
            UserId = reportDto.UserId,
            Longitude = reportDto.Longitude,
            Latitude = reportDto.Latitude,
            Date = reportDto.Date,
            Type = reportDto.Type,
            Status = reportDto.Status
        };
        try
        {
            _dbContext.Reports.Add(report);
            await _dbContext.SaveChangesAsync();
            Console.WriteLine($"Saved: {myQueueItem}");
            log.LogInformation($"Saved to database: {myQueueItem}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving to the database: {ex.Message}");
            log.LogError($"Error saving to the database: {ex.Message}");
        }
    }
}
