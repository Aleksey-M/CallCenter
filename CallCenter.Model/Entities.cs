using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CallCenter.Model
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
        [Required(ErrorMessage = "Поле обязательное для заполнения")]
        [DisplayName("Дата звонка")]
        [DisplayFormat(DataFormatString = "{0:f}")]
        public DateTime CallDate { get; set; }
        [Range(0, 1000000, ErrorMessage = "От 0 до 1 000 000")]
        [DisplayName("Стоимость заказа")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double? OrderCost { get; set; }
        [DisplayName("Отчет")]
        [Required(ErrorMessage = "Поле обязательное для заполнения")]
        [MaxLength(500, ErrorMessage = "Длина строки должна быть до 500 символов")]
        public string CallReport { get; set; }

        public Guid? PersonId { get; set; }

        public bool Equals(Call other)
        {
            return CallDate == other.CallDate &&
                OrderCost == other.OrderCost &&
                CallReport.Equals(other.CallReport);
        }

        public Call Clone() => new Call
        {
            CallId = this.CallId,
            PersonId = this.PersonId,
            CallDate = this.CallDate,
            CallReport = this.CallReport,
            OrderCost = this.OrderCost
        };
    }

    public enum Gender { All, Male, Female }

    public class Person : IEquatable<Person> 
    {        
        public Guid PersonId { get; set; }
        [Required(ErrorMessage = "Поле обязательное для заполнения")]
        [DisplayName("Имя"), StringLength(50)]
        public string FirstName { get; set; }
        [DisplayName("Фамилия"), StringLength(50)]
        public string LastName { get; set; }
        [DisplayName("Отчество"), StringLength(50)]
        public string Patronymic { get; set; }
        [DisplayName("Дата рождения")]
        [DisplayFormat(DataFormatString = "{0:D}")]
        public DateTime? BirthDate { get; set; }
        [DisplayName("Пол")]
        public Gender Gender { get; set; }
        [Required(ErrorMessage = "Поле обязательное для заполнения")]
        [DisplayName("Номер телефона"), StringLength(20)]        
        public string PhoneNumber { get; set; }

        public List<Call> Calls { get; set; }

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
            return obj is Person person && Equals(person);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public Person Clone() => new Person
        {
            PersonId = this.PersonId,
            BirthDate = this.BirthDate,
            FirstName = this.FirstName,
            LastName = this.LastName,
            Patronymic = this.Patronymic,
            Gender = this.Gender,
            PhoneNumber = this.PhoneNumber,
            Calls = null
        };
    }
        
}
