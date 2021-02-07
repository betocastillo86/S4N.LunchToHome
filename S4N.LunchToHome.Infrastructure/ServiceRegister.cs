using Microsoft.Extensions.DependencyInjection;
using S4N.LunchToHome.Application.Common;
using S4N.LunchToHome.Application.Common.Devices;
using S4N.LunchToHome.Infrastructure.Devices;
using S4N.LunchToHome.Infrastructure.Helpers;
using S4N.LunchToHome.Infrastructure.Persistence;

namespace S4N.LunchToHome.Infrastructure
{
    public static class ServiceRegister
    {
        public static void RegisterInfrastructure(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IFileHelper, FileHelper>();
            serviceCollection.AddScoped<IDroneFlyingDriver, DroneFlyingDriver>();
            serviceCollection.AddScoped(typeof(IRepository<>), typeof(MemoryRepository<>));
        }
    }
}