using System;
using System.Collections.Generic;

namespace CallCenter.NgWebApp.Data
{
    public class CallsDistinctEqualityComparer : IEqualityComparer<Call>
    {
        public bool Equals(Call x, Call y)
        {
            return x.PersonId == y.PersonId;
        }

        public int GetHashCode(Call obj)
        {
            return obj.PersonId.GetHashCode();
        }
    }

    public class PersonsListItem
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
    }

    public class Call : IEquatable<Call>
    {
        public Guid CallId { get; set; }
        public DateTime CallDate { get; set; }
        public double? OrderCost { get; set; }
        public string CallReport { get; set; }

        public Guid? PersonId { get; set; }
        public Person Person { get; set; }

        public bool Equals(Call other)
        {
            return CallDate == other.CallDate &&
                OrderCost == other.OrderCost &&
                CallReport.Equals(other.CallReport);
        }
    }

    public enum Gender { All, Male, Femaile }

    public class Person : IEquatable<Person> 
    {
        public Guid PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public DateTime? BirthDate { get; set; }
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; }

        public IList<Call> Calls { get; set; }

        public bool Equals(Person other)
        {            
            return 
                FirstName.Equals(other.FirstName) &&
                LastName.Equals(other.LastName) &&
                Patronymic.Equals(other.Patronymic) &&
                BirthDate == other.BirthDate &&
                Gender == other.Gender &&
                PhoneNumber.Equals(other.PhoneNumber);
        }

        public override bool Equals(object obj)
        {
            var person = obj as Person;
            return person == null ? false : Equals(person);            
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
        
}
