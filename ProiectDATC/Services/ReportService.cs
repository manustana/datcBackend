using Microsoft.EntityFrameworkCore;
using ProiectDATC.Models;
using Azure.Messaging.ServiceBus;
using System.Text.Json;
using System.Text;
public class ReportService
{
    private readonly AppDbContext _context;
    private readonly string _serviceBusConnectionString;
    private readonly string _queueName;

    public ReportService(AppDbContext context)
    {
        _context = context;
        _serviceBusConnectionString = "Endpoint=sb://proiectdatc.servicebus.windows.net/;SharedAccessKeyName=root;SharedAccessKey=N8Rfc27yh3RzzuwPGIKAVQSP1uuV81Xbi+ASbPomOUc=;EntityPath=proiectdatc";
        _queueName = "proiectdatc";
    }

    public async Task<int> CreateReportAsync(ReportDto report)
    {
        await SendMessageToQueueAsync(report);
        return 0;
    }

    public async Task<List<Report>> GetAllReportsAsync()
    {
        return await _context.Reports.ToListAsync();
    }

    public async Task<Report> GetReportByIdAsync(int id)
    {
        return await _context.Reports.FindAsync(id);
    }

    public async Task UpdateReportAsync(int id, ReportDto model)
    {
        var existingReport = await _context.Reports.FindAsync(id);

        if (existingReport == null)
        {
            throw new ArgumentException("Report not found");
        }

        existingReport.UserId = model.UserId;
        existingReport.Longitude = model.Longitude;
        existingReport.Latitude = model.Latitude;
        existingReport.Date = model.Date;
        existingReport.Type = model.Type;
        existingReport.Status = "DONE";

        var user = await _context.Users.FindAsync(model.UserId);
        if (user != null)
        {
            user.Points += 10;
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteReportAsync(int id)
    {
        var report = await _context.Reports.FindAsync(id);

        if (report == null)
        {
            throw new ArgumentException("Report not found");
        }

        _context.Reports.Remove(report);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Report>> GetReportsByUserIdAsync(int userId)
    {
        return await _context.Reports
            .Where(r => r.UserId == userId)
            .ToListAsync();
    }

    private async Task SendMessageToQueueAsync(ReportDto report)
    {
        await using var client = new ServiceBusClient(_serviceBusConnectionString);
        var sender = client.CreateSender(_queueName);

        var jsonString = JsonSerializer.Serialize(report);

        var message = new ServiceBusMessage
        {
            Body = new BinaryData(Encoding.UTF8.GetBytes(jsonString))
        };

        await sender.SendMessageAsync(message);
    }
}
