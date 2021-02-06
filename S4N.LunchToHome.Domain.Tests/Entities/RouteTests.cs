using NUnit.Framework;
using S4N.LunchToHome.Domain.Exceptions;
using S4N.LunchToHome.Domain.ValueObjects;

namespace S4N.LunchToHome.Domain.Tests.Entities
{
    [TestFixture]
    public class RouteTests
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("aid")]
        [TestCase("BCEF")]
        [TestCase("AAAADDDIIB")]
        [TestCase("BAAAADDDII")]
        [TestCase("AAAABDDDII")]
        public void CreateRoute_InvalidRoutes_ThrowInvalidPathException(string path)
        {
            Assert.Throws<InvalidPathException>(() => Route.CreateRoute(path));
        }

        [Test]
        [TestCase("A")]
        [TestCase("D")]
        [TestCase("I")]
        [TestCase("ADI")]
        [TestCase("AAADDDIII")]
        public void CreateRoute_ValidRoute_SamePath(string path)
        {
            var route = Route.CreateRoute(path);

            Assert.AreEqual(path, route.Path);
        }
    }
}