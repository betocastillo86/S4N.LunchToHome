using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using S4N.LunchToHome.Application.Common;
using S4N.LunchToHome.Application.Common.Exceptions;
using S4N.LunchToHome.Application.Deliveries.Services;
using S4N.LunchToHome.Domain.Entities;
using S4N.LunchToHome.Domain.ValueObjects;

namespace S4N.LunchToHome.Application.Deliveries.Commands.SendDelivery
{
    public class SendDeliveryCommandHandler : IRequestHandler<SendDeliveryCommand, bool>
    {
        private readonly IMovementService movementService;

        private readonly IRepository<Delivery> deliveryRepository;

        private readonly IPublisher publisher;

        private readonly ILogger<SendDeliveryCommandHandler> logger;

        public SendDeliveryCommandHandler(
            IMovementService movementService,
            IRepository<Delivery> deliveryRepository,
            IPublisher publisher,
            ILogger<SendDeliveryCommandHandler> logger)
        {
            this.movementService = movementService;
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

            var position = new Position(0, 0, Domain.Enums.Direction.North);

            foreach (var route in delivery.Routes)
            {
                try
                {
                    position = await this.movementService.FlyPathAsync(position, route.Path);
                    await this.publisher.Publish(new OnRouteFinishedEvent { Delivery = delivery, NewPosition = position });
                }
                catch (ProcessPathException)
                {
                    // do something to control delivery
                    this.logger.LogError($"Can't process route {route.Path} on position {position}");
                    await this.movementService.ReturnToRestaurantAsync();
                    return false;
                }
            }

            await this.publisher.Publish(new OnDeliverySentEvent { Position = position });

            this.logger.LogInformation($"Deliver succesfully sent {request.DeliveryId}");

            return true;
        }
    }
}