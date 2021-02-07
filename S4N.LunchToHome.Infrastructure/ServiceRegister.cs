using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using S4N.LunchToHome.Application.Common;
using S4N.LunchToHome.Application.Common.Devices;
using S4N.LunchToHome.Application.Common.Settings;
using S4N.LunchToHome.Infrastructure.Devices;
using S4N.LunchToHome.Infrastructure.Helpers;
using S4N.LunchToHome.Infrastructure.Persistence;
using S4N.LunchToHome.Infrastructure.Settings;

namespace S4N.LunchToHome.Infrastructure
{
    public static class ServiceRegister
    {
        public static void RegisterInfrastructure(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IFileHelper, FileHelper>();
            serviceCollection.AddScoped<IGeneralSettings, GeneralSettings>();
            serviceCollection.AddScoped<IDroneFlyingDriver, DroneFlyingDriver>();
            serviceCollection.AddScoped(typeof(IRepository<>), typeof(MemoryRepository<>));
            serviceCollection.AddMediatR(Assembly.GetExecutingAssembly());
        }
    }
}