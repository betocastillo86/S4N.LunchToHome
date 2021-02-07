using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace S4N.LunchToHome.Application
{
    public static class ServiceRegister
    {
        public static void RegisterApplication(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddMediatR(Assembly.GetExecutingAssembly());
        }
    }
}