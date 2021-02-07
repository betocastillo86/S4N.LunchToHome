using MediatR;
using S4N.LunchToHome.Domain.ValueObjects;

namespace S4N.LunchToHome.Application.Deliveries.Commands.SendDelivery
{
    public class OnDeliverySentEvent : INotification
    {
        public Position Position { get; set; }
    }
}