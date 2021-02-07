using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using S4N.LunchToHome.Application.Common;
using S4N.LunchToHome.Application.Common.Extensions;
using S4N.LunchToHome.Application.Common.Settings;
using S4N.LunchToHome.Application.Deliveries.Commands.SendDelivery;
using S4N.LunchToHome.Domain.Entities;
using S4N.LunchToHome.Infrastructure.Helpers;

namespace S4N.LunchToHome.Infrastructure.Subscribers
{
    public class TraceDeliverySubscriber : INotificationHandler<OnRouteFinishedEvent>
    {
        private readonly IFileHelper fileHelper;

        private readonly IGeneralSettings generalSettings;

        private readonly IRepository<Drone> droneRepository;

        private readonly ILogger<TraceDeliverySubscriber> logger;

        public TraceDeliverySubscriber(
            IFileHelper fileHelper,
            IGeneralSettings generalSettings,
            IRepository<Drone> droneRepository,
            ILogger<TraceDeliverySubscriber> logger)
        {
            this.fileHelper = fileHelper;
            this.generalSettings = generalSettings;
            this.droneRepository = droneRepository;
            this.logger = logger;
        }

        public Task Handle(OnRouteFinishedEvent notification, CancellationToken cancellationToken)
        {
            var drone = this.droneRepository.Items.FirstOrDefault(c => c.Id == notification.Delivery.DroneId);

            if (drone == null)
            {
                this.logger.LogWarning($"Drone ({notification.Delivery.DroneId}) not found for delivery ({notification.Delivery.Id})");
                return Task.CompletedTask;
            }

            var path = string.Format(this.generalSettings.OutputFilePath, drone.Name);

            this.fileHelper.WriteContentOnFile(path, notification.NewPosition.ToText(), this.generalSettings.OutputHeaderText);

            return Task.CompletedTask;
        }
    }
}