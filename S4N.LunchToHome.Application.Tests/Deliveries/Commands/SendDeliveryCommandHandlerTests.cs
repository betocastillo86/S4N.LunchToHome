﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using S4N.LunchToHome.Application.Common;
using S4N.LunchToHome.Application.Common.Devices;
using S4N.LunchToHome.Application.Common.Exceptions;
using S4N.LunchToHome.Application.Deliveries.Commands.SendDelivery;
using S4N.LunchToHome.Domain.Entities;
using S4N.LunchToHome.Domain.ValueObjects;

namespace S4N.LunchToHome.Application.Tests.Deliveries.Commands
{
    [TestFixture]
    public class SendDeliveryCommandHandlerTests
    {
        private SendDeliveryCommandHandler handler;

        private SendDeliveryCommand request;

        private IList<Delivery> deliveries;

        private Mock<IRepository<Delivery>> deliveryRepository;

        private Mock<IDroneFlyingDriver> droneFlyingDriver;

        private Mock<IPublisher> publisher;

        private CancellationToken cancel;

        [SetUp]
        public void SetUp()
        {
            this.cancel = new CancellationToken();

            this.request = new SendDeliveryCommand();

            this.deliveries = new List<Delivery>();

            this.deliveryRepository = new Mock<IRepository<Delivery>>();
            this.droneFlyingDriver = new Mock<IDroneFlyingDriver>();
            this.publisher = new Mock<IPublisher>();

            this.deliveryRepository.SetupGet(c => c.Items)
                .Returns(() => this.deliveries.AsQueryable());

            this.handler = new SendDeliveryCommandHandler(this.droneFlyingDriver.Object, this.deliveryRepository.Object, this.publisher.Object);
        }

        [Test]
        public void Handle_DeliveryNotFound_ThrowNotFoundException()
        {
            this.request.DeliveryId = Guid.NewGuid();

            Assert.ThrowsAsync<NotFoundException>(() => this.handler.Handle(this.request, this.cancel));
        }

        [Test]
        public async Task Handle_DeliveryFoundWithTwoRoutes_OnRouteFinishedEventTwiceAndOnDeliverySentEventOnceAndReturnTrue()
        {
            var finalPosition = new Position(10, 10, Domain.Enums.Direction.East);
            this.request.DeliveryId = Guid.NewGuid();

            this.droneFlyingDriver.Setup(c => c.FlyPathAsync(It.IsAny<Position>(), "A"))
                .ReturnsAsync(() => finalPosition);

            var delivery = new Delivery
            {
                Id = this.request.DeliveryId,
                DroneId = Guid.NewGuid(),
                Routes = new List<Route>
                {
                    Route.CreateRoute("A"),
                    Route.CreateRoute("A")
                }
            };

            this.deliveries.Add(delivery);

            var result = await this.handler.Handle(this.request, this.cancel);

            this.publisher.Verify(c => c.Publish(It.Is<OnRouteFinishedEvent>(c => c.NewPosition.Equals(finalPosition)), this.cancel), Times.Exactly(2));
            this.publisher.Verify(c => c.Publish(It.Is<OnDeliverySentEvent>(c => c.Position.Equals(finalPosition)), this.cancel), Times.Once);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task Handle_FlyingFails_DronReturnsToHomeAndReturnFalseAndDonCallEventDeliverySent()
        {
            this.request.DeliveryId = Guid.NewGuid();

            this.droneFlyingDriver.Setup(c => c.FlyPathAsync(It.IsAny<Position>(), It.IsAny<string>()))
                .ThrowsAsync(new DroneFlyingException());

            var delivery = new Delivery
            {
                Id = this.request.DeliveryId,
                DroneId = Guid.NewGuid(),
                Routes = new List<Route>
                {
                    Route.CreateRoute("A"),
                    Route.CreateRoute("A")
                }
            };

            this.deliveries.Add(delivery);

            var result = await this.handler.Handle(this.request, this.cancel);

            this.droneFlyingDriver.Verify(c => c.ReturnToRestaurantAsync(), Times.Once);
            this.publisher.Verify(c => c.Publish(It.IsAny<OnDeliverySentEvent>(), this.cancel), Times.Never);

            Assert.IsFalse(result);
        }
    }
}