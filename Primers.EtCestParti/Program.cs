using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Primers.EtCestParti
{
    public class Program
    {
        private static readonly Random Random = new Random(DateTime.Now.TimeOfDay.Milliseconds);


        private static void Main(string[] args)
        {
            var input = Console.In.ReadToEnd();
            var room = Solve(input);
            Improve(room, 100);
            
            var score = room.Score;

            // var results = Enumerable.Range(0, 1000).AsParallel().Select(x => Solve(input)).OrderByDescending(x => x.Score).ToList();
        }

        private static void Improve(Room room, int maxIterations)
        {
            var iterations = 0;
            var changesMade = false;
            do {
                foreach (var swap in GenerateSwaps(room)) {
                    var currentScore = swap.Item1.Score + swap.Item2.Score;
                    var alternativeScore = swap.Item1.ScoreIfPersonSwappedWith(swap.Item2.Person)
                        + swap.Item2.ScoreIfPersonSwappedWith(swap.Item1.Person);
                    if (alternativeScore > currentScore) {
                        var p2 = swap.Item2.Person;
                        swap.Item2.Person = swap.Item1.Person;
                        swap.Item1.Person = p2;
                        changesMade = true;
                        break;
                    }
                }
                iterations++;
            } while (changesMade && iterations < maxIterations);
        }

        private static IEnumerable<Tuple<Spot, Spot>> GenerateSwaps(Room room)
        {
            var spots = room.ToList();
            for (var i = 0; i < spots.Count; i++) {
                for (var j = 0; j < i; j++) {
                    yield return new Tuple<Spot, Spot>(spots[i], spots[j]);
                }
            }
        }

        public static Room Solve(string input)
        {
            using (var reader = new StringReader(input)) {
                var room = ReadRoom(reader);
                var numberOfPeople = Convert.ToInt32(reader.ReadLine());
                var people = Enumerable.Range(0, numberOfPeople)
                    .Select(x => new Person(x))
                    .ToDictionary(x => x.Id, x => x);
                ReadAppreciations(reader, people);
                //            var spotDistribution = room.GroupBy(x => x.Neighbours.Keys.Count).ToDictionary(x => x.Key, x => x.Count()).OrderBy(x => x.Key);
                //            var distribution = people.Values.GroupBy(x => x.NumberOfFriends).ToDictionary(x => x.Key, x => x.Count()).OrderBy(x => x.Key);
                //            Distribute(people, room, Convert.ToInt32(args[0]));
                Distribute2(people, room);
                //            var score = room.Score;
                return room;
            }
//            PrintOutput(room, Console.Out);
        }

        private static void Distribute2(Dictionary<int, Person> people, Room room)
        {
            var availablePeople = new HashSet<Person>(people.Values);
            var availableSpots = new HashSet<Spot>(room.Where(x => x.Person == null));

            while (availableSpots.Any()) {
                var spot = PickSpot(availableSpots);
                spot.Person = PickPerson(availablePeople, spot.Neighbours.Values.Where(x => x.Person != null).Select(x => x.Person));

                var nextSpots = new HashSet<Spot>(spot.Neighbours.Values.Where(x => x.Person == null));
                while (nextSpots.Any()) {
                    var next = new HashSet<Spot>();
                    foreach (var s in Distribute2(availablePeople, nextSpots)) {
                        next.Add(s);
                    }
                    foreach (var s in nextSpots) {
                        availableSpots.Remove(s);
                    }
                    nextSpots = next;
                }
            }
        }

        public static IEnumerable<Spot> Distribute2(ISet<Person> availablePeople, ISet<Spot> nextSpots)
        {
            foreach (var spot in nextSpots) {
                spot.Person = PickPerson(availablePeople, spot.Neighbours.Values.Where(x => x.Person != null).Select(x => x.Person));

                foreach (var s in spot.Neighbours.Values.Where(x => x.Person == null)) {
                    yield return s;
                }
            }
        }

        private static Person PickPerson(ISet<Person> people, IEnumerable<Person> neighbours)
        {
            var potentialPeople = people.OrderByDescending(x => neighbours.Count(y => y.Likes(x))).ToArray();
            var person = potentialPeople.FirstOrDefault();
            people.Remove(person);
            return person;
        }

        private static Spot PickSpot(ICollection<Spot> spots)
        {
            var spot = spots.ElementAt(Random.Next(spots.Count));
            spots.Remove(spot);
            return spot;
        }

        private static void PrintOutput(Room room, TextWriter writer)
        {
            foreach (var spot in room) {
                writer.WriteLine($"{spot.Person.Id},{spot.Position.X},{spot.Position.Y}");
            }
        }

        private static void Distribute(IDictionary<int, Person> people, Room room, int maxIterations)
        {
            var index = 0;
            var peopleByNumberFriends = people.Values.OrderBy(x => x.NumberOfFriends).ToArray();
            var spotsByNumberNeighbours = room.OrderBy(x => x.Neighbours.Keys.Count).ToArray();

            foreach (var spot in spotsByNumberNeighbours) {
                spot.Person = peopleByNumberFriends[index++];
            }

            var iterations = 0;
            var changeMade = false;
            do {
                var awfulSpots = room.OrderBy(x => x.Score).ToList();
                var spot1 = awfulSpots.First();
                foreach (var spot2 in awfulSpots.Skip(1)) {
                    var currentScore = spot1.Score + spot2.Score;
                    var p1 = spot1.Person;
                    var p2 = spot2.Person;
                    spot1.Person = p2;
                    spot2.Person = p1;

                    if (spot1.Score + spot2.Score > currentScore) {
                        changeMade = true;
                        break;
                    }

                    // restore
                    spot1.Person = p1;
                    spot2.Person = p2;
                }
                iterations++;
            } while (changeMade && iterations < maxIterations);
        }

        private static void ReadAppreciations(TextReader reader, IDictionary<int, Person> people)
        {
            var line = reader.ReadLine();
            while (!String.IsNullOrEmpty(line)) {
                var parts = line.Split(',').Select(x => Convert.ToInt32(x)).ToArray();
                people[parts[0]].SetLikes(people[parts[1]]);
                people[parts[1]].SetLikes(people[parts[0]]);
                line = reader.ReadLine();
            }
        }

        private static Room ReadRoom(TextReader reader)
        {
            var parts = reader.ReadLine().Split(',').Select(x => Convert.ToInt32(x)).ToArray();
            var numberObstacles = Convert.ToInt32(reader.ReadLine());
            var positions = new List<Position>();
            for (var i = 0; i < numberObstacles; i++) {
                positions.Add(ReadPosition(reader));
            }
            return new Room(parts[0], parts[1], positions);
        }

        private static Position ReadPosition(TextReader reader)
        {
            var parts = reader.ReadLine().Split(',').Select(x => Convert.ToInt32(x)).ToArray();
            return new Position(parts[0], parts[1]);
        }
    }
}
