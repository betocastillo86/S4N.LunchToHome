using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using S4N.LunchToHome.Application;
using S4N.LunchToHome.Application.Common;
using S4N.LunchToHome.Domain.Entities;
using S4N.LunchToHome.Domain.ValueObjects;
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
        }

        private static void RegisterDependencies()
        {
            var serviceCollection = new ServiceCollection();

            Config = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .Build();

            serviceCollection.AddScoped<IConfiguration>((service) => Config);

            serviceCollection.RegisterInfrastructure();

            serviceCollection.RegisterApplication();

            TheServiceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}