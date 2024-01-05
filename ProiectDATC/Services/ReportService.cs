using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProiectDATC.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

    // public async Task<int> CreateReportAsync(Report report)
    // {
    //     _context.Reports.Add(report);
    //     await _context.SaveChangesAsync();
    //     return report.Id;
    // }

    public async Task<int> CreateReportAsync(ReportDto report)
    {
        // Send the report data to Azure Service Bus queue
        await SendMessageToQueueAsync(report);

        // Note: You might want to return something meaningful, depending on your application
        return 0;
    }

    public async Task UpdateReportAsync(int id, ReportDto model)
    {
        var existingReport = await _context.Reports.FindAsync(id);

        if (existingReport == null)
        {
            throw new ArgumentException("Report not found");
        }

        // Update properties based on your requirements
        existingReport.UserId = model.UserId;
        existingReport.Longitude = model.Longitude;
        existingReport.Latitude = model.Latitude;
        existingReport.Date = model.Date;
        existingReport.Type = model.Type;
        existingReport.Status = model.Status;

        // Send the updated report data to Azure Service Bus queue
        await SendMessageToQueueAsyncUpdateReport(existingReport);
    }
    public async Task<List<Report>> GetAllReportsAsync()
    {
        return await _context.Reports.ToListAsync();
    }

    public async Task<Report> GetReportByIdAsync(int id)
    {
        return await _context.Reports.FindAsync(id);
    }

    // public async Task UpdateReportAsync(int id, Report model)
    // {
    //     var existingReport = await _context.Reports.FindAsync(id);

    //     if (existingReport == null)
    //     {
    //         throw new ArgumentException("Report not found");
    //     }

    //     // Update properties based on your requirements
    //     existingReport.UserId = model.UserId;
    //     existingReport.Longitude = model.Longitude;
    //     existingReport.Latitude = model.Latitude;
    //     existingReport.Date = model.Date;
    //     existingReport.Type = model.Type;
    //     existingReport.Status = model.Status;

    //     await _context.SaveChangesAsync();
    // }

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

    private async Task SendMessageToQueueAsyncUpdateReport(Report report)
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
