using S4N.LunchToHome.Domain.Enums;
using S4N.LunchToHome.Domain.ValueObjects;

namespace S4N.LunchToHome.Application.Common.Extensions
{
    public static class PositionExtensions
    {
        public static string ToText(this Position position)
        {
            return $"({position.X}, {position.Y}) dirección {position.Direction.ToText()}";
        }

        public static string ToText(this Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return "Norte";

                case Direction.South:
                    return "Sur";

                case Direction.West:
                    return "Occidente";

                case Direction.East:
                    return "Oriente";

                default:
                    return string.Empty;
            }
        }
    }
}