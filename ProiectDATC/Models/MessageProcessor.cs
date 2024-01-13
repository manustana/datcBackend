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
                var jsonString = Encoding.UTF8.GetString(message.Body.ToArray());
                var report = JsonSerializer.Deserialize<Report>(jsonString);

                await _dbContext.Reports.AddAsync(report);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
        }
    }

}