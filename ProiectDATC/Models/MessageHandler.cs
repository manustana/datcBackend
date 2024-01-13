using Azure.Messaging.ServiceBus;

namespace ProiectDATC.Models
{
    public class MessageHandler
    {
        private readonly ServiceBusProcessor _processor;
        private readonly MessageProcessor _messageProcessor;

        public MessageHandler(string connectionString, string queueName, AppDbContext dbContext)
        {
            var processorOptions = new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 1,
                AutoCompleteMessages = false
            };

            var client = new ServiceBusClient(connectionString);
            _messageProcessor = new MessageProcessor(dbContext);
            _processor = client.CreateProcessor(queueName, processorOptions);
            _processor.ProcessMessageAsync += ProcessMessageHandler;
        }

        private async Task ProcessMessageHandler(ProcessMessageEventArgs args)
        {
            await _messageProcessor.ProcessMessageAsync(args.Message);

            await args.CompleteMessageAsync(args.Message);
        }

        public async Task StartAsync()
        {
            await _processor.StartProcessingAsync();
        }

        public async Task StopAsync()
        {
            await _processor.StopProcessingAsync();
        }
    }

}