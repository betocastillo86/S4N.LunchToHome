using MediatR;
using S4N.LunchToHome.Domain.Entities;
using S4N.LunchToHome.Domain.ValueObjects;

namespace S4N.LunchToHome.Application.Deliveries.Commands.SendDelivery
{
    public class OnRouteFinishedEvent : INotification
    {
        public Delivery Delivery { get; set; }

        public Position NewPosition { get; set; }
    }
}