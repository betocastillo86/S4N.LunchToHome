using System.Threading.Tasks;
using NUnit.Framework;
using S4N.LunchToHome.Application.Common.Exceptions;
using S4N.LunchToHome.Domain.Enums;
using S4N.LunchToHome.Domain.ValueObjects;
using S4N.LunchToHome.Infrastructure.Devices;

namespace S4N.LunchToHome.Infrastructure.Tests.Devices
{
    [TestFixture]
    public class DroneFlyingDriverTests
    {
        private DroneFlyingDriver service;

        [SetUp]
        public void SetUp()
        {
            this.service = new DroneFlyingDriver();
        }

        [Test]
        public void FlyPathAsync_InvalidPath_ThrowDroneFlyingException()
        {
            var position = new Position(0, 0, Direction.East);
            var path = "z";

            Assert.ThrowsAsync<DroneFlyingException>(() => this.service.FlyPathAsync(position, path));
        }

        [Test]
        public async Task FlyPathAsync_MoveForwardToNorth_IncrementY()
        {
            var position = new Position(0, 0, Direction.North);
            var path = "A";

            var newPosition = await this.service.FlyPathAsync(position, path);

            Assert.AreEqual(0, newPosition.X);
            Assert.AreEqual(1, newPosition.Y);
            Assert.AreEqual(Direction.North, newPosition.Direction);
        }

        [Test]
        public async Task FlyPathAsync_MoveForwardToSouth_DecrementY()
        {
            var position = new Position(0, 0, Direction.South);
            var path = "A";

            var newPosition = await this.service.FlyPathAsync(position, path);

            Assert.AreEqual(0, newPosition.X);
            Assert.AreEqual(-1, newPosition.Y);
            Assert.AreEqual(Direction.South, newPosition.Direction);
        }

        [Test]
        public async Task FlyPathAsync_MoveForwardToEast_IncrementX()
        {
            var position = new Position(0, 0, Direction.East);
            var path = "A";

            var newPosition = await this.service.FlyPathAsync(position, path);

            Assert.AreEqual(1, newPosition.X);
            Assert.AreEqual(0, newPosition.Y);
            Assert.AreEqual(Direction.East, newPosition.Direction);
        }

        [Test]
        public async Task FlyPathAsync_MoveForwardToWest_DecrementX()
        {
            var position = new Position(0, 0, Direction.West);
            var path = "A";

            var newPosition = await this.service.FlyPathAsync(position, path);

            Assert.AreEqual(-1, newPosition.X);
            Assert.AreEqual(0, newPosition.Y);
            Assert.AreEqual(Direction.West, newPosition.Direction);
        }

        [Test]
        [TestCase(Direction.North, Direction.East)]
        [TestCase(Direction.East, Direction.South)]
        [TestCase(Direction.South, Direction.West)]
        [TestCase(Direction.West, Direction.North)]
        public async Task FlyPathAsync_TurnRight_MoveToValidDirection(Direction from, Direction to)
        {
            var position = new Position(0, 0, from);
            var path = "D";

            var newPosition = await this.service.FlyPathAsync(position, path);

            Assert.AreEqual(0, newPosition.X);
            Assert.AreEqual(0, newPosition.Y);
            Assert.AreEqual(to, newPosition.Direction);
        }

        [Test]
        [TestCase(Direction.North, Direction.West)]
        [TestCase(Direction.East, Direction.North)]
        [TestCase(Direction.South, Direction.East)]
        [TestCase(Direction.West, Direction.South)]
        public async Task FlyPathAsync_TurnLeft_MoveToValidDirection(Direction from, Direction to)
        {
            var position = new Position(0, 0, from);
            var path = "I";

            var newPosition = await this.service.FlyPathAsync(position, path);

            Assert.AreEqual(0, newPosition.X);
            Assert.AreEqual(0, newPosition.Y);
            Assert.AreEqual(to, newPosition.Direction);
        }

        [Test]
        [TestCase("AAAAIAA", 0, 0, Direction.North, -2, 4, Direction.North)]
        [TestCase("DDDAIAD", -2, 4, Direction.North, -3, 3, Direction.South)]
        [TestCase("AAIADAD", -3, 3, Direction.South, -4, 2, Direction.East)]
        public async Task FlyPathAsync_FollowS4NTestCases_MoveToValidDirection(string path, int xFrom, int yFrom, Direction dirFrom, int xTo, int yTo, Direction dirTo)
        {
            var from = new Position(xFrom, yFrom, dirFrom);

            var newPosition = await this.service.FlyPathAsync(from, path);

            Assert.AreEqual(xTo, newPosition.X);
            Assert.AreEqual(yTo, newPosition.Y);
            Assert.AreEqual(dirTo, newPosition.Direction);
        }

        [Test]
        [TestCase("AAAAIAA", 0, 0, Direction.North, -2, 4, Direction.West)]
        [TestCase("DDDAIAD", -2, 4, Direction.West, -1, 3, Direction.South)]
        [TestCase("AAIADAD", -1, 3, Direction.South, 0, 0, Direction.West)]
        public async Task FlyPathAsync_S4NTestCasesFixed_MoveToValidDirection(string path, int xFrom, int yFrom, Direction dirFrom, int xTo, int yTo, Direction dirTo)
        {
            var from = new Position(xFrom, yFrom, dirFrom);

            var newPosition = await this.service.FlyPathAsync(from, path);

            Assert.AreEqual(xTo, newPosition.X);
            Assert.AreEqual(yTo, newPosition.Y);
            Assert.AreEqual(dirTo, newPosition.Direction);
        }
    }
}