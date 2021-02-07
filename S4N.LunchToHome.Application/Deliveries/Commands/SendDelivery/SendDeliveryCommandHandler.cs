using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using S4N.LunchToHome.Application.Common;
using S4N.LunchToHome.Application.Common.Devices;
using S4N.LunchToHome.Application.Common.Exceptions;
using S4N.LunchToHome.Domain.Entities;
using S4N.LunchToHome.Domain.ValueObjects;

namespace S4N.LunchToHome.Application.Deliveries.Commands.SendDelivery
{
    public class SendDeliveryCommandHandler : IRequestHandler<SendDeliveryCommand, bool>
    {
        private readonly IDroneFlyingDriver droneFlyingDriver;

        private readonly IRepository<Delivery> deliveryRepository;

        private readonly IPublisher publisher;

        private readonly ILogger<SendDeliveryCommandHandler> logger;

        public SendDeliveryCommandHandler(
            IDroneFlyingDriver droneFlyingDriver,
            IRepository<Delivery> deliveryRepository,
            IPublisher publisher,
            ILogger<SendDeliveryCommandHandler> logger)
        {
            this.droneFlyingDriver = droneFlyingDriver;
            this.deliveryRepository = deliveryRepository;
            this.publisher = publisher;
            this.logger = logger;
        }

        public async Task<bool> Handle(SendDeliveryCommand request, CancellationToken cancellationToken)
        {
            var delivery = this.deliveryRepository.Items.FirstOrDefault(c => c.Id == request.DeliveryId);

            if (delivery == null)
            {
                throw new NotFoundException(request.DeliveryId);
            }

            this.logger.LogInformation($"Process send delivery {request.DeliveryId}");

            var initialPosition = new Position(0, 0, Domain.Enums.Direction.North);

            foreach (var route in delivery.Routes)
            {
                try
                {
                    initialPosition = await this.droneFlyingDriver.FlyPathAsync(initialPosition, route.Path);
                    await this.publisher.Publish(new OnRouteFinishedEvent { Delivery = delivery, NewPosition = initialPosition });
                }
                catch (DroneFlyingException)
                {
                    // do something to control delivery
                    this.logger.LogError($"Can't process route {route.Path} on position {initialPosition}");
                    await this.droneFlyingDriver.ReturnToRestaurantAsync();
                    return false;
                }
            }

            await this.publisher.Publish(new OnDeliverySentEvent { Position = initialPosition });

            this.logger.LogInformation($"Deliver succesfully sent {request.DeliveryId}");

            return true;
        }
    }
}