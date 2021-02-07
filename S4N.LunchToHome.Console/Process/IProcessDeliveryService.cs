using System.Threading.Tasks;

namespace S4N.LunchToHome.ConsoleApplication
{
    public interface IProcessDeliveryService
    {
        Task ProcessAsync();
    }
}