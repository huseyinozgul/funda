using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

using MakelaarsList.Abstractions;
using MakelaarsList.Services;

namespace MakelaarsList
{
    class Program
    {
        public static IConfigurationRoot configuration;

        static void Main(string[] args)
        {

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                Log.Information("Start");
                MainAsync(args).Wait();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        static async Task MainAsync(string[] args)
        {

            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            IFeedService feedService = serviceProvider.GetService<FeedService>();

            try
            {
                Log.Information("Getting Makelaars");

                await feedService.ShowMakelaars(onlyHasGarden: false, top: 10);
                // await feedService.ShowMakelaars(onlyHasGarden: true, top: 10);

                Log.Information("Ending...");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Error running service");
                throw ex;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(LoggerFactory.Create(builder =>
            {
                builder.AddSerilog(dispose: true);
            }));

            serviceCollection.AddLogging();

            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            serviceCollection.AddSingleton(configuration);
            serviceCollection.AddTransient<FeedServiceClient>();
            serviceCollection.AddTransient<FeedService>();

        }
    }
}
