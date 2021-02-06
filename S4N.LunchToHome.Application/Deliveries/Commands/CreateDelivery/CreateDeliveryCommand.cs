using System;
using System.Collections.Generic;
using MediatR;
using S4N.LunchToHome.Application.Deliveries.Models;

namespace S4N.LunchToHome.Application.Deliveries.Commands.CreateDelivery
{
    public class CreateDeliveryCommand : IRequest<Guid>
    {
        public Guid DroneId { get; set; }

        public IList<RouteModel> Routes { get; set; } = new List<RouteModel>();
    }
}