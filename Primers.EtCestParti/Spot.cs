using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Primers.EtCestParti
{
    [DebuggerDisplay("{Position} = {Person}")]
    public class Spot
    {
        public Position Position { get; }

        public Person Person { get; set; }

        public IDictionary<Direction, Spot> Neighbours = new Dictionary<Direction, Spot>(4);

        public Spot(Position position)
        {
            Position = position;
        }

        public int Score {
            get {
                return Neighbours.Values.Count(neighbour => Person.Friends.Contains(neighbour.Person));
            }
        }

        public int ScoreIfPersonSwappedWith(Person other)
        {
            return Neighbours.Values.Count(neighbour => other.Friends.Contains(neighbour.Person));
        }

        public Spot this[Direction direction] {
            get { return Neighbours[direction]; }
            set {
                if (!Neighbours.ContainsKey(direction) || Neighbours[direction] != value) {
                    Neighbours[direction] = value;
                    value[DirectionUtils.OppositeDirection(direction)] = this;
                }
            }
        }
    }
}