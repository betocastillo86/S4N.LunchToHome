using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using S4N.LunchToHome.Application.Common;
using S4N.LunchToHome.Domain.Entities;
using S4N.LunchToHome.Domain.ValueObjects;

namespace S4N.LunchToHome.Application.Deliveries.Commands.CreateDelivery
{
    public class CreateDeliveryCommandHandler : IRequestHandler<CreateDeliveryCommand, Guid>
    {
        private readonly IRepository<Delivery> deliveryRespoitory;

        public CreateDeliveryCommandHandler(
            IRepository<Delivery> deliveryRespoitory)
        {
            this.deliveryRespoitory = deliveryRespoitory;
        }

        public async Task<Guid> Handle(CreateDeliveryCommand request, CancellationToken cancellationToken)
        {
            var delivery = new Delivery
            {
                Id = Guid.NewGuid(),
                DroneId = request.DroneId,
                Routes = request.Routes.Select(c => Route.CreateRoute(c.Path)).ToList()
            };

            await this.deliveryRespoitory.InsertAsync(delivery);

            return delivery.Id;
        }
    }
}