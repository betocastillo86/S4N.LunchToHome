using System.Collections.Generic;
using S4N.LunchToHome.Domain.ValueObjects;

namespace S4N.LunchToHome.Domain.Entities
{
    public class Delivery : BaseEntity
    {
        public int DronId { get; set; }

        public IList<Route> Routes { get; set; } = new List<Route>();
    }
}