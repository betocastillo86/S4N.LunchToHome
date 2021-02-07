using System.Threading.Tasks;
using S4N.LunchToHome.Domain.ValueObjects;

namespace S4N.LunchToHome.Application.Deliveries.Services
{
    public interface IMovementService
    {
        Task<Position> FlyPathAsync(Position initialPosition, string path);

        Task ReturnToRestaurantAsync();
    }
}