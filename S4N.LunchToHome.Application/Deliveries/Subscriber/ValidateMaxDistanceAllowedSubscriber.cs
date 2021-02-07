using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using S4N.LunchToHome.Application.Common.Devices;
using S4N.LunchToHome.Application.Common.Exceptions;
using S4N.LunchToHome.Application.Common.Settings;
using S4N.LunchToHome.Application.Deliveries.Commands.SendDelivery;
using S4N.LunchToHome.Domain.ValueObjects;

namespace S4N.LunchToHome.Application.Deliveries.Subscriber
{
    public class ValidateMaxDistanceAllowedSubscriber : INotificationHandler<OnRouteFinishedEvent>
    {
        private readonly IGeneralSettings generalSettings;

        private readonly IDroneFlyingDriver droneFlyingDriver;

        private readonly ILogger<ValidateMaxDistanceAllowedSubscriber> logger;

        public ValidateMaxDistanceAllowedSubscriber(
            IGeneralSettings generalSettings,
            IDroneFlyingDriver droneFlyingDriver,
            ILogger<ValidateMaxDistanceAllowedSubscriber> logger)
        {
            this.generalSettings = generalSettings;
            this.droneFlyingDriver = droneFlyingDriver;
            this.logger = logger;
        }

        public async Task Handle(OnRouteFinishedEvent notification, CancellationToken cancellationToken)
        {
            if (!this.ValidateMaxDistanceAllowed(notification.NewPosition))
            {
                await this.droneFlyingDriver.ReturnToRestaurantAsync();

                var message = $"Drone {notification.Delivery.DroneId} exceed position {notification.NewPosition}";

                this.logger.LogWarning(message);
                throw new MaxDistanceAllowedExceededException(message);
            }
        }

        private bool ValidateMaxDistanceAllowed(Position position)
        {
            return Math.Abs(position.X) <= this.generalSettings.MaxDistanceAllowed && Math.Abs(position.Y) <= this.generalSettings.MaxDistanceAllowed;
        }
    }
}