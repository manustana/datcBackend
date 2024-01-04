using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;

namespace ProiectDATC.Models
{
    public class MessageProcessor
    {
        private readonly AppDbContext _dbContext;

        public MessageProcessor(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task ProcessMessageAsync(ServiceBusReceivedMessage message)
        {
            try
            {
                // Deserialize the message content (assuming it's JSON)
                var jsonString = Encoding.UTF8.GetString(message.Body.ToArray());
                var report = JsonSerializer.Deserialize<Report>(jsonString);

                // Save the report to the database
                await _dbContext.Reports.AddAsync(report);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle exceptions, log, or retry logic as needed
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
        }
    }

}