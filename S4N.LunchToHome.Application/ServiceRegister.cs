﻿using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using S4N.LunchToHome.Application.Common.Behaviours;

namespace S4N.LunchToHome.Application
{
    public static class ServiceRegister
    {
        public static void RegisterApplication(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddMediatR(Assembly.GetExecutingAssembly());

            serviceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            serviceCollection.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}