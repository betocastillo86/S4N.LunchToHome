using S4N.LuchToHome.Domain.Enums;

namespace S4N.LuchToHome.Domain.ValueObjects
{
    public class Position
    {
        public Position(long x, long y, Direction direction)
        {
            this.X = x;
            this.Y = y;
            this.Direction = direction;
        }

        public long X { get; private set; }

        public long Y { get; private set; }

        public Direction Direction { get; private set; }
    }
}