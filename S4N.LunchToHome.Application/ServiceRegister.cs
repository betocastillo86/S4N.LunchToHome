using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using S4N.LunchToHome.Application.Common.Behaviours;
using S4N.LunchToHome.Application.Deliveries.Services;

namespace S4N.LunchToHome.Application
{
    public static class ServiceRegister
    {
        public static void RegisterApplication(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddMediatR(Assembly.GetExecutingAssembly());

            serviceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            serviceCollection.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            serviceCollection.AddScoped<IMovementService, MovementService>();
        }
    }
}