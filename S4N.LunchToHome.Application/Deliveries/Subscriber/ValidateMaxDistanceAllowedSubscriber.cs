using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using S4N.LunchToHome.Application.Common.Exceptions;
using S4N.LunchToHome.Application.Common.Settings;
using S4N.LunchToHome.Application.Deliveries.Commands.SendDelivery;
using S4N.LunchToHome.Application.Deliveries.Services;
using S4N.LunchToHome.Domain.ValueObjects;

namespace S4N.LunchToHome.Application.Deliveries.Subscriber
{
    public class ValidateMaxDistanceAllowedSubscriber : INotificationHandler<OnRouteFinishedEvent>
    {
        private readonly IGeneralSettings generalSettings;

        private readonly IMovementService movementService;

        private readonly ILogger<ValidateMaxDistanceAllowedSubscriber> logger;

        public ValidateMaxDistanceAllowedSubscriber(
            IGeneralSettings generalSettings,
            IMovementService movementService,
            ILogger<ValidateMaxDistanceAllowedSubscriber> logger)
        {
            this.generalSettings = generalSettings;
            this.movementService = movementService;
            this.logger = logger;
        }

        public async Task Handle(OnRouteFinishedEvent notification, CancellationToken cancellationToken)
        {
            if (!this.ValidateMaxDistanceAllowed(notification.NewPosition))
            {
                await this.movementService.ReturnToRestaurantAsync();

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