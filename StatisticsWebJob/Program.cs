using Microsoft.EntityFrameworkCore; // Add this line
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var hostBuilder = new HostBuilder();
        
        hostBuilder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        });

        hostBuilder.ConfigureServices((context, services) =>
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection"));
            });
        });

        hostBuilder.ConfigureWebJobs((context, builder) =>
        {
            builder.AddServiceBus(options =>
            {
                options.MaxConcurrentCalls = 1;
            });
        });

        var host = hostBuilder.Build();

        using (host)
        {
            await host.RunAsync();
        }
    }
}
