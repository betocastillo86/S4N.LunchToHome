using System.Threading.Tasks;
using S4N.LunchToHome.Domain.ValueObjects;

namespace S4N.LunchToHome.Application.Common.Devices
{
    public interface IDroneFlyingDriver
    {
        Task<Position> FlyPathAsync(Position initialPosition, string path);

        Task ReturnToRestaurantAsync();
    }
}