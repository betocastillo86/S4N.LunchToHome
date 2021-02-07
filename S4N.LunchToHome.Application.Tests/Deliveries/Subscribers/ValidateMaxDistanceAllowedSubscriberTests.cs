using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using S4N.LunchToHome.Application.Common.Devices;
using S4N.LunchToHome.Application.Common.Exceptions;
using S4N.LunchToHome.Application.Common.Settings;
using S4N.LunchToHome.Application.Deliveries.Commands.SendDelivery;
using S4N.LunchToHome.Application.Deliveries.Subscriber;
using S4N.LunchToHome.Domain.Enums;
using S4N.LunchToHome.Domain.ValueObjects;

namespace S4N.LunchToHome.Application.Tests.Deliveries.Subscribers
{
    [TestFixture]
    public class ValidateMaxDistanceAllowedSubscriberTests
    {
        private ValidateMaxDistanceAllowedSubscriber subscriber;

        private Mock<IGeneralSettings> generalSettings;

        private Mock<IDroneFlyingDriver> droneFlyingDriver;

        private Mock<ILogger<ValidateMaxDistanceAllowedSubscriber>> logger;

        private OnRouteFinishedEvent eventObject;

        private CancellationToken cancel;

        [SetUp]
        public void SetUp()
        {
            this.cancel = new CancellationToken();
            this.eventObject = new OnRouteFinishedEvent() { Delivery = new Domain.Entities.Delivery { DroneId = Guid.NewGuid() } };

            this.generalSettings = new Mock<IGeneralSettings>();
            this.droneFlyingDriver = new Mock<IDroneFlyingDriver>();
            this.logger = new Mock<ILogger<ValidateMaxDistanceAllowedSubscriber>>();

            this.subscriber = new ValidateMaxDistanceAllowedSubscriber(this.generalSettings.Object, this.droneFlyingDriver.Object, this.logger.Object);
        }

        [Test]
        [TestCase(9)]
        [TestCase(10)]
        [TestCase(-9)]
        [TestCase(-10)]
        public void Handle_LessOrEqualDistanceThanAllowedWithX_ContinueWithoutThrowingException(int x)
        {
            var limit = 10;
            this.generalSettings.SetupGet(c => c.MaxDistanceAllowed).Returns(limit);
            this.eventObject.NewPosition = new Position(x, 0, Direction.East);

            Assert.DoesNotThrowAsync(() => this.subscriber.Handle(this.eventObject, this.cancel));
        }

        [Test]
        [TestCase(9)]
        [TestCase(10)]
        [TestCase(-9)]
        [TestCase(-10)]
        public void Handle_LessOrEqualDistanceThanAllowedWithY_ContinueWithoutThrowingException(int y)
        {
            var limit = 10;
            this.generalSettings.SetupGet(c => c.MaxDistanceAllowed).Returns(limit);
            this.eventObject.NewPosition = new Position(0, y, Direction.East);

            Assert.DoesNotThrowAsync(() => this.subscriber.Handle(this.eventObject, this.cancel));
        }

        [Test]
        [TestCase(11)]
        [TestCase(-11)]
        public void Handle_GreaterDistanceThanAllowedWithX_ThrowExceptionAndReturnHome(int x)
        {
            var limit = 10;
            this.generalSettings.SetupGet(c => c.MaxDistanceAllowed).Returns(limit);
            this.eventObject.NewPosition = new Position(x, 0, Direction.East);

            Assert.ThrowsAsync<MaxDistanceAllowedExceededException>(() => this.subscriber.Handle(this.eventObject, this.cancel));
            this.droneFlyingDriver.Verify(c => c.ReturnToRestaurantAsync(), Times.Once);
        }

        [Test]
        [TestCase(11)]
        [TestCase(-11)]
        public void Handle_GreaterDistanceThanAllowedWithY_ThrowExceptionAndReturnHome(int y)
        {
            var limit = 10;
            this.generalSettings.SetupGet(c => c.MaxDistanceAllowed).Returns(limit);
            this.eventObject.NewPosition = new Position(0, y, Direction.East);

            Assert.ThrowsAsync<MaxDistanceAllowedExceededException>(() => this.subscriber.Handle(this.eventObject, this.cancel));
            this.droneFlyingDriver.Verify(c => c.ReturnToRestaurantAsync(), Times.Once);
        }
    }
}