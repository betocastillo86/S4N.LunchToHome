using System.Threading.Tasks;

namespace S4N.LunchToHome.Application.Common.Devices
{
    public interface IDroneDriver
    {
        Task Move();

        Task Turn(bool right);
    }
}