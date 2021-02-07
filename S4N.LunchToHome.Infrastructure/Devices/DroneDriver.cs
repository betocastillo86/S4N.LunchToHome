using System.Threading.Tasks;
using S4N.LunchToHome.Application.Common.Devices;

namespace S4N.LunchToHome.Infrastructure.Devices
{
    public class DroneDriver : IDroneDriver
    {
        public Task Move()
        {
            Task.Delay(100).Wait();
            return Task.CompletedTask;
        }

        public Task Turn(bool right)
        {
            return Task.CompletedTask;
        }
    }
}