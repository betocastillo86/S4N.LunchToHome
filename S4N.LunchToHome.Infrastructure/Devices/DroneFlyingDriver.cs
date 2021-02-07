using System.Threading.Tasks;
using S4N.LunchToHome.Application.Common.Devices;
using S4N.LunchToHome.Application.Common.Exceptions;
using S4N.LunchToHome.Domain.Enums;
using S4N.LunchToHome.Domain.ValueObjects;

namespace S4N.LunchToHome.Infrastructure.Devices
{
    public class DroneFlyingDriver : IDroneFlyingDriver
    {
        public async Task<Position> FlyPathAsync(Position initialPosition, string path)
        {
            return await this.MoveDroneToNextPosition(initialPosition, path);
        }

        public Task ReturnToRestaurantAsync()
        {
            return Task.CompletedTask;
        }

        private async Task<Position> MoveDroneToNextPosition(Position initialPosition, string path)
        {
            var nextStep = path[0];
            Position newPosition;

            switch (nextStep)
            {
                case 'A':
                    newPosition = this.MoveForward(initialPosition);
                    await Task.Delay(1);
                    break;

                case 'I':
                    newPosition = this.Turn(initialPosition, false);
                    break;

                case 'D':
                    newPosition = this.Turn(initialPosition, true);
                    break;

                default:
                    throw new DroneFlyingException($"Invalid path {path}");
            }

            if (path.Length == 1)
            {
                return newPosition;
            }
            else
            {
                return await MoveDroneToNextPosition(newPosition, path.Substring(1));
            }
        }

        private Position MoveForward(Position initialPosition)
        {
            var x = initialPosition.X;
            var y = initialPosition.Y;

            switch (initialPosition.Direction)
            {
                case Direction.North:
                    y++;
                    break;

                case Direction.South:
                    y--;
                    break;

                case Direction.West:
                    x--;
                    break;

                case Direction.East:
                    x++;
                    break;

                default:
                    throw new DroneFlyingException($"Invalid direction on position {initialPosition}");
            }

            return new Position(x, y, initialPosition.Direction);
        }

        private Position Turn(Position initialPosition, bool right)
        {
            Direction newDirection;

            switch (initialPosition.Direction)
            {
                case Direction.North:
                    newDirection = right ? Direction.East : Direction.West;
                    break;

                case Direction.South:
                    newDirection = right ? Direction.West : Direction.East;
                    break;

                case Direction.West:
                    newDirection = right ? Direction.North : Direction.South;
                    break;

                case Direction.East:
                    newDirection = right ? Direction.South : Direction.North;
                    break;

                default:
                    throw new DroneFlyingException($"Invalid direction on position {initialPosition}");
            }

            return new Position(initialPosition.X, initialPosition.Y, newDirection);
        }
    }
}