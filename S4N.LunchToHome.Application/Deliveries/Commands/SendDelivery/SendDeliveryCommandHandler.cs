﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
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

        public SendDeliveryCommandHandler(
            IDroneFlyingDriver droneFlyingDriver,
            IRepository<Delivery> deliveryRepository,
            IPublisher publisher)
        {
            this.droneFlyingDriver = droneFlyingDriver;
            this.deliveryRepository = deliveryRepository;
            this.publisher = publisher;
        }

        public async Task<bool> Handle(SendDeliveryCommand request, CancellationToken cancellationToken)
        {
            var delivery = this.deliveryRepository.Items.FirstOrDefault(c => c.Id == request.DeliveryId);

            if (delivery == null)
            {
                throw new NotFoundException(request.DeliveryId);
            }

            var initialPosition = new Position(0, 0, Domain.Enums.Direction.North);

            foreach (var route in delivery.Routes)
            {
                try
                {
                    initialPosition = await this.droneFlyingDriver.FlyPathAsync(initialPosition, route.Path);
                    await this.publisher.Publish(new OnRouteFinishedEvent { Delivery = delivery, NewPosition = initialPosition });
                }
                catch (DroneFlyingException e)
                {
                    // do something to control delivery
                    await this.droneFlyingDriver.ReturnToRestaurantAsync();
                    return false;
                }
            }

            await this.publisher.Publish(new OnDeliverySentEvent { Position = initialPosition });

            return true;
        }
    }
}