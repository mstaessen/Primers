using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Primers.EtCestParti
{
    [DebuggerDisplay("({X}, {Y})")]
    public struct Position
    {
        public int X { get; }

        public int Y { get; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Position other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            return obj is Position && Equals((Position) obj);
        }

        public override int GetHashCode()
        {
            unchecked {
                return (X * 397) ^ Y;
            }
        }

        public static bool operator ==(Position p1, Position p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(Position p1, Position p2)
        {
            return !p1.Equals(p2);
        }

        public Position Move(Direction direction, int size = 1)
        {
            switch (direction) {
                case Direction.North:
                    return new Position(X, Y + size);
                case Direction.East:
                    return new Position(X + size, Y);
                case Direction.South:
                    return new Position(X, Y - size);
                case Direction.West:
                    return new Position(X - size, Y);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public static IEnumerable<Position> Generate(int width, int height)
        {
            for (var x = 0; x < width; x++) {
                for (var y = 0; y < height; y++) {
                    yield return new Position(x, y);
                }
            }
        }
    }
}