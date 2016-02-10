using System.Collections.Generic;
using System.Diagnostics;

namespace Primers.EtCestParti
{
    [DebuggerDisplay("Person {Id}")]
    public class Person
    {
        public int Id { get; }

        public ISet<Person> Friends { get; } = new HashSet<Person>();

        public int NumberOfFriends => Friends.Count;

        public Person(int id)
        {
            Id = id;
        }

        public bool Likes(Person other)
        {
            return Friends.Contains(other);
        }

        public void SetLikes(Person other)
        {
            Friends.Add(other);
        }
    }
}