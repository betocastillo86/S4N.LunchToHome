using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using S4N.LunchToHome.Application;
using S4N.LunchToHome.Application.Common;
using S4N.LunchToHome.ConsoleApplication.Process;
using S4N.LunchToHome.Domain.Entities;
using S4N.LunchToHome.Infrastructure;

namespace S4N.LunchToHome.ConsoleApplication
{
    internal class Program
    {
        private static IServiceProvider TheServiceProvider;

        private static IConfigurationRoot Config;

        private static void Main(string[] args)
        {
            RegisterDependencies();

            StartSeeding();

            ProcessDeliveries();
        }

        private static async void ProcessDeliveries()
        {
            try
            {
                var processDeliveryService = TheServiceProvider.GetService<IProcessDeliveryService>();
                await processDeliveryService.ProcessAsync();
            }
            catch (Exception e)
            {
                var logger = TheServiceProvider.GetService<ILogger<Program>>();
                logger.LogError(e, "Error processing deliveries");
            }
        }

        private static void StartSeeding()
        {
            var drones = TheServiceProvider.GetService<IRepository<Drone>>();

            for (int i = 1; i <= 20; i++)
            {
                drones.InsertAsync(new Drone { Id = Guid.NewGuid(), Name = i.ToString("00") });
            }
        }

        private static void RegisterDependencies()
        {
            var serviceCollection = new ServiceCollection();

            Config = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .Build();

            serviceCollection.AddLogging(c =>
            {
                c.AddConsole();
            });

            serviceCollection.AddSingleton<IConfiguration>(Config);
            serviceCollection.AddScoped<IProcessDeliveryService, ProcessDeliveryService>();

            serviceCollection.RegisterInfrastructure();

            serviceCollection.RegisterApplication();

            TheServiceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}