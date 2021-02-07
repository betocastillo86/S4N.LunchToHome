using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using S4N.LunchToHome.Application.Common;
using S4N.LunchToHome.Application.Common.Extensions;
using S4N.LunchToHome.Application.Common.Settings;
using S4N.LunchToHome.Application.Deliveries.Commands.SendDelivery;
using S4N.LunchToHome.Domain.Entities;
using S4N.LunchToHome.Domain.Enums;
using S4N.LunchToHome.Domain.ValueObjects;
using S4N.LunchToHome.Infrastructure.Subscribers;

namespace S4N.LunchToHome.Infrastructure.Tests.Subscribers
{
    [TestFixture]
    public class TraceDeliverySubscriberTests
    {
        private TraceDeliverySubscriber subscriber;

        private Mock<Helpers.IFileHelper> fileHelper;

        private Mock<IGeneralSettings> generalSettings;

        private Mock<IRepository<Drone>> droneRepository;

        private Mock<ILogger<TraceDeliverySubscriber>> logger;

        private IList<Drone> drones;

        private OnRouteFinishedEvent eventRaised;

        private CancellationToken cancel;

        private string headerText = "== Reporte de entregas ==";

        [SetUp]
        public void SetUp()
        {
            this.eventRaised = new OnRouteFinishedEvent();
            this.drones = new List<Drone>();
            this.cancel = new CancellationToken();

            this.fileHelper = new Mock<Helpers.IFileHelper>();
            this.generalSettings = new Mock<IGeneralSettings>();
            this.droneRepository = new Mock<IRepository<Drone>>();
            this.logger = new Mock<ILogger<TraceDeliverySubscriber>>();

            this.droneRepository.SetupGet(c => c.Items)
                .Returns(() => this.drones.AsQueryable());

            this.generalSettings.SetupGet(c => c.OutputFilePath)
                .Returns("out{0}.txt");

            this.generalSettings.SetupGet(c => c.OutputHeaderText)
                .Returns(this.headerText);

            this.subscriber = new TraceDeliverySubscriber(
                this.fileHelper.Object,
                this.generalSettings.Object,
                this.droneRepository.Object,
                this.logger.Object);
        }

        [Test]
        public void Handle_NotFoundDrone_LogWarning()
        {
            this.eventRaised.Delivery = new Delivery { DroneId = Guid.NewGuid() };

            this.drones.Add(new Drone { Id = Guid.NewGuid() });

            this.subscriber.Handle(this.eventRaised, this.cancel);

            this.logger.Verify(
              c => c.Log(
                  LogLevel.Warning,
                  It.IsAny<EventId>(),
                  It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("not found for delivery")),
                  It.IsAny<Exception>(),
                  (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
              Times.Once);
        }

        [Test]
        public void Handle_ExistentDrone_WriteInfoOnFileWithValidFileAndText()
        {
            this.eventRaised.Delivery = new Delivery { DroneId = Guid.NewGuid() };
            this.eventRaised.NewPosition = new Position(0, 0, Direction.North);

            var drone = new Drone { Id = this.eventRaised.Delivery.DroneId, Name = "01" };
            this.drones.Add(drone);

            var text = this.eventRaised.NewPosition.ToText();
            var filename = $"out{drone.Name}.txt";

            this.subscriber.Handle(this.eventRaised, this.cancel);

            this.fileHelper.Verify(c => c.WriteContentOnFile(filename, text, this.headerText));
        }
    }
}