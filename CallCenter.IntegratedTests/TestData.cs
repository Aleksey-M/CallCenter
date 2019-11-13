using CallCenter.Back.Data;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace CallCenter
{
    public static class TestData
    {
        public static List<Person> Get3TestPersons(bool generateId = true, bool uniqNames = true)
        {
            var persons = new List<Person>();
            //
            var person = new Person() {
                    PersonId = generateId ? Guid.NewGuid() : Guid.Empty,
                    BirthDate = DateTime.Parse("20.05.1990", CultureInfo.GetCultureInfo("ru-RU")).AddYears(DateTime.Now.Year - 1990 - 27),//27
                    FirstName = uniqNames ? Guid.NewGuid().ToString().Substring(0, 8) : "Алена",
                    Patronymic = uniqNames ? Guid.NewGuid().ToString().Substring(0, 8) : "Викторовна",
                    LastName = uniqNames ? Guid.NewGuid().ToString().Substring(0, 8) : "Михайлюк",
                    Gender = Gender.Female,
                    PhoneNumber = "+380964521256",
                };            
            persons.Add(person);
            //---------------------
            person = new Person()
            {
                PersonId = generateId ? Guid.NewGuid() : Guid.Empty,
                BirthDate = DateTime.Parse("18.03.1998", CultureInfo.GetCultureInfo("ru-RU")).AddYears(DateTime.Now.Year - 1998 - 19), //19
                FirstName = uniqNames ? Guid.NewGuid().ToString().Substring(0, 8) : "Евгений",
                Patronymic = uniqNames ? Guid.NewGuid().ToString().Substring(0, 8) : "Павлович",
                LastName = uniqNames ? Guid.NewGuid().ToString().Substring(0, 8) : "Мостовой",
                Gender = Gender.Male,
                PhoneNumber = "+380502486363",               
            };            
            persons.Add(person);
            //---------------------
            person = new Person()
            {
                PersonId = generateId ? Guid.NewGuid() : Guid.Empty,
                BirthDate = DateTime.Parse("7.11.2004", CultureInfo.GetCultureInfo("ru-RU")).AddYears(DateTime.Now.Year - 2004 - 13), //13
                FirstName = uniqNames ? Guid.NewGuid().ToString().Substring(0, 8) : "Ольга",
                Patronymic = uniqNames ? Guid.NewGuid().ToString().Substring(0, 8) : "Владимировна",
                LastName = uniqNames ? Guid.NewGuid().ToString().Substring(0, 8) : "Кожемякина",
                Gender = Gender.All,
                PhoneNumber = "+380954562312",                
            };            
            persons.Add(person);

            return persons;
        }

        public static List<Call> Get9TestCalls()
        {
            var calls = new List<Call>
            {
                new Call()
                {
                    CallId = Guid.Empty,
                    CallDate = DateTime.Now.AddDays(-1),
                    OrderCost = 0,
                    CallReport = "Звонок прошел хорошо"
                },
                new Call()
                {
                    CallId = Guid.Empty,
                    CallDate = DateTime.Now.AddDays(-7),
                    OrderCost = 0,
                    CallReport = "Опреатор вне зоны доступа"
                },
                new Call()
                {
                    CallId = Guid.Empty,
                    CallDate = DateTime.Now.AddDays(-5),
                    OrderCost = 199.55,
                    CallReport = "Был сделан заказ"
                },
                new Call()
                {
                    CallId = Guid.Empty,
                    CallDate = DateTime.Now.AddDays(-15),
                    OrderCost = 55.12,
                    CallReport = "Часть заказа была с браком"
                },
                new Call()
                {
                    CallId = Guid.Empty,
                    CallDate = DateTime.Now.AddDays(-10),
                    OrderCost = 0,
                    CallReport = "С 12 до 16 - неудобное время"
                },
                new Call()
                {
                    CallId = Guid.Empty,
                    CallDate = DateTime.Now.AddDays(-8),
                    OrderCost = 230.00,
                    CallReport = "Повторный заказ 55"
                },
                new Call()
                {
                    CallId = Guid.Empty,
                    CallDate = DateTime.Now.AddDays(-16),
                    OrderCost = 0,
                    CallReport = "Отмена повторного заказа"
                },
                new Call()
                {
                    CallId = Guid.Empty,
                    CallDate = DateTime.Now.AddDays(-20),
                    OrderCost = 330.85,
                    CallReport = "Заказ из новой коллекции"
                },
                new Call()
                {
                    CallId = Guid.Empty,
                    CallDate = DateTime.Now.AddDays(-18),
                    OrderCost = 5,
                    CallReport = "Уточнение заказа и замена товара"
                }
            };
            return calls;
        }
    }
}
