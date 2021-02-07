using System;
using MediatR;

namespace S4N.LunchToHome.Application.Deliveries.Commands.SendDelivery
{
    public class SendDeliveryCommand : IRequest<bool>
    {
        public Guid DeliveryId { get; set; }
    }
}