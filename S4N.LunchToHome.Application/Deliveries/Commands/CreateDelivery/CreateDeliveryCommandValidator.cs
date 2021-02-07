using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using S4N.LunchToHome.Application.Common;
using S4N.LunchToHome.Application.Common.Settings;
using S4N.LunchToHome.Application.Deliveries.Models;
using S4N.LunchToHome.Domain.Entities;

namespace S4N.LunchToHome.Application.Deliveries.Commands.CreateDelivery
{
    public class CreateDeliveryCommandValidator : AbstractValidator<CreateDeliveryCommand>
    {
        private readonly IRepository<Drone> droneRepository;

        private readonly IGeneralSettings generalSettings;

        public CreateDeliveryCommandValidator(
            IRepository<Drone> droneRepository,
            IGeneralSettings generalSettings)
        {
            this.droneRepository = droneRepository;
            this.generalSettings = generalSettings;

            this.RuleFor(c => c)
                .NotNull();

            this.RuleFor(c => c.DroneId)
                .NotEqual(Guid.Empty)
                .Must(this.DroneShouldExist).WithMessage("Drone doesn't exist");

            this.RuleFor(c => c.Routes)
                .Must(this.MinItems).WithMessage("Should have at least one route")
                .Must(this.MaxItems).WithMessage($"Should have maximum {this.generalSettings.MaxRoutesPerDrone} routes");

            this.When(c => c.Routes != null && c.Routes.Count > 0, () =>
            {
                this.RuleForEach(c => c.Routes)
                    .SetValidator(new RouteModelValidator());
            });
        }

        private bool MaxItems(IList<RouteModel> routes)
        {
            return routes != null && routes.Count <= this.generalSettings.MaxRoutesPerDrone;
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