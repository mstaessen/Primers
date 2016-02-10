using System;

namespace Primers.EtCestParti
{
    public static class DirectionUtils
    {
        public static Direction[] Directions = {Direction.North, Direction.East, Direction.South, Direction.West};

        public static Direction OppositeDirection(Direction direction)
        {
            switch (direction) {
                case Direction.North:
                    return Direction.South;
                case Direction.East:
                    return Direction.West;
                case Direction.South:
                    return Direction.North;
                case Direction.West:
                    return Direction.East;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }
    }
}