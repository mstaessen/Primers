using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Primers.EtCestParti
{
    public class Room : IEnumerable<Spot>
    {
        private readonly IDictionary<Position, Spot> spots;

        public int Width { get; }

        public int Height { get; }

        public int Score {
            get {
                var score = 0;
                foreach (var spot in this) {
                    var east = spot.Position.Move(Direction.East);
                    if (HasSpot(east) && Likes(this[spot.Position].Person, this[east].Person)) {
                        score++;
                    }

                    var north = spot.Position.Move(Direction.North);
                    if (HasSpot(north) && Likes(this[spot.Position].Person, this[north].Person)) {
                        score++;
                    }
                }
                return score;
            }
        }

        private static bool Likes(Person p1, Person p2)
        {
            if (p1 == null) {
                return false;
            }
            if (p2 == null) {
                return false;
            }
            return p1.Likes(p2);
        }

        public Room(int width, int height, IEnumerable<Position> obstaclePositions)
        {
            Width = width;
            Height = height;
            spots = new Dictionary<Position, Spot>(width * height);
            foreach (var position in Position.Generate(width, height).Except(obstaclePositions)) {
                spots[position] = new Spot(position);
                foreach (var direction in DirectionUtils.Directions) {
                    var neighbouringPosition = position.Move(direction);
                    if (HasSpot(neighbouringPosition)) {
                        var neighbour = spots[neighbouringPosition];
                        if (neighbour != null) {
                            this[position][direction] = neighbour;
                        }
                    }
                }
            }
        }

        public bool HasSpot(int x, int y)
        {
            return HasSpot(new Position(x, y));
        }

        public bool HasSpot(Position position)
        {
            return spots.ContainsKey(position);
        }

        public Spot this[int x, int y] {
            get { return spots[new Position(x, y)]; }
            set { spots[new Position(x, y)] = value; }
        }

        public Spot this[Position position] {
            get { return spots[position]; }
            private set { spots[position] = value; }
        }

        public IEnumerator<Spot> GetEnumerator()
        {
            return spots.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}