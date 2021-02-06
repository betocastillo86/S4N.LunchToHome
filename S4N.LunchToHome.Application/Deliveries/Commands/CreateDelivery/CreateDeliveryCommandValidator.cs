using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using S4N.LunchToHome.Application.Common;
using S4N.LunchToHome.Application.Deliveries.Models;
using S4N.LunchToHome.Domain.Entities;

namespace S4N.LunchToHome.Application.Deliveries.Commands.CreateDelivery
{
    public class CreateDeliveryCommandValidator : AbstractValidator<CreateDeliveryCommand>
    {
        private readonly IRepository<Drone> droneRepository;

        public CreateDeliveryCommandValidator(IRepository<Drone> droneRepository)
        {
            this.droneRepository = droneRepository;

            this.RuleFor(c => c)
                .NotNull();

            this.RuleFor(c => c.DroneId)
                .NotEqual(Guid.Empty)
                .Must(this.DroneShouldExist).WithMessage("Drone doesn't exist");

            this.RuleFor(c => c.Routes)
                .Must(this.MinItems).WithMessage("Should have at least one route");

            this.When(c => c.Routes != null && c.Routes.Count > 0, () =>
            {
                this.RuleForEach(c => c.Routes)
                    .SetValidator(new RouteModelValidator());
            });
        }

        private bool MinItems(IList<RouteModel> routes)
        {
            return routes != null && routes.Count > 0;
        }

        private bool DroneShouldExist(Guid id)
        {
            return this.droneRepository.Items.Any(c => c.Id.Equals(id));
        }
    }
}