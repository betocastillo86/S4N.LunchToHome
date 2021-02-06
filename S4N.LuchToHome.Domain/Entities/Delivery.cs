using System.Collections.Generic;
using S4N.LuchToHome.Domain.ValueObjects;

namespace S4N.LuchToHome.Domain.Entities
{
    public class Delivery : BaseEntity
    {
        public int DronId { get; set; }

        public IList<Route> Routes { get; set; }
    }
}