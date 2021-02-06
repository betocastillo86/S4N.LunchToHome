using S4N.LuchToHome.Domain.ValueObjects;

namespace S4N.LuchToHome.Domain.Entities
{
    public class Dron : BaseEntity
    {
        public Position CurrentPosition { get; set; }
    }
}