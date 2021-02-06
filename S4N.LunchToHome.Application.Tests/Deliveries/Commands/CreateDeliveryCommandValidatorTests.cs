using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using S4N.LunchToHome.Application.Common;
using S4N.LunchToHome.Application.Deliveries.Commands.CreateDelivery;
using S4N.LunchToHome.Application.Deliveries.Models;
using S4N.LunchToHome.Domain.Entities;

namespace S4N.LunchToHome.Application.Tests.Deliveries.Commands
{
    [TestFixture]
    public class CreateDeliveryCommandValidatorTests
    {
        private CreateDeliveryCommandValidator validator;

        private CreateDeliveryCommand model;

        private IList<Drone> drones;

        private Mock<IRepository<Drone>> droneRepository = new Mock<IRepository<Drone>>();

        [SetUp]
        public void SetUp()
        {
            this.drones = new List<Drone>();

            this.droneRepository = new Mock<IRepository<Drone>>();

            this.validator = new CreateDeliveryCommandValidator(this.droneRepository.Object);

            this.model = new CreateDeliveryCommand();

            this.droneRepository.SetupGet(c => c.Items)
                .Returns(() => this.drones.AsQueryable());
        }

        [Test]
        public void Validate_EmptyModel_ShouldFail()
        {
            var result = this.validator.TestValidate(this.model);

            result.ShouldHaveValidationErrorFor("DroneId");
            result.ShouldHaveValidationErrorFor("Routes");
        }

        [Test]
        public void Validate_InvalidRoute_ShouldFail()
        {
            this.model.Routes.Add(new RouteModel { Path = "xyz" });

            var result = this.validator.TestValidate(this.model);

            result.ShouldHaveValidationErrorFor("Routes[0].Path");
        }

        [Test]
        public void Validate_ValidRoute_ShouldPass()
        {
            this.model.Routes.Add(new RouteModel { Path = "AID" });

            var result = this.validator.TestValidate(this.model);

            result.ShouldNotHaveValidationErrorFor("Routes");
            result.ShouldNotHaveValidationErrorFor("Routes[0].Path");
        }

        [Test]
        public void Validate_NotExistentDrone_ShouldFail()
        {
            this.model.DroneId = Guid.NewGuid();

            var result = this.validator.TestValidate(this.model);

            result.ShouldHaveValidationErrorFor("DroneId")
                .WithErrorMessage("Drone doesn't exist");
        }

        [Test]
        public void Validate_ExistentDrone_ShouldPass()
        {
            this.model.DroneId = Guid.NewGuid();

            this.drones.Add(new Drone { Id = this.model.DroneId });

            var result = this.validator.TestValidate(this.model);

            result.ShouldNotHaveValidationErrorFor("DroneId");
        }
    }
}