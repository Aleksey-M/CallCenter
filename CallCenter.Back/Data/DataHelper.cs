using System;
using System.Linq;

namespace CallCenter.Back.Data
{
    public static class DataHelper
    {
        public static void AddTestData(DataBaseContext context)
        {
            context.Persons.Add(new Person
            {
                PersonId = Guid.Empty,
                BirthDate = DateTime.Now.Date.AddYears(-27).AddMonths(5).AddDays(20), //27
                FirstName = "Алена",
                Patronymic = "Викторовна",
                LastName = "Михайлюк",
                Gender = Gender.Female,
                PhoneNumber = "+380964521256",
            });
            context.Persons.Add(new Person
            {
                PersonId = Guid.Empty,
                BirthDate = DateTime.Now.Date.AddYears(-19).AddMonths(3).AddDays(18), //19
                FirstName = "Евгений",
                Patronymic = "Павлович",
                LastName = "Мостовой",
                Gender = Gender.Male,
                PhoneNumber = "+380502486363",
            });
            context.Persons.Add(new Person()
            {
                PersonId = Guid.Empty,
                BirthDate = DateTime.Now.Date.AddYears(-13).AddMonths(11).AddDays(7), //13
                FirstName = "Ольга",
                Patronymic = "Владимировна",
                LastName = "Кожемякина",
                Gender = Gender.All,
                PhoneNumber = "+380954562312",
            });
            context.Persons.Add(new Person()
            {
                PersonId = Guid.Empty,
                BirthDate = DateTime.Now.Date.AddYears(-17).AddMonths(6).AddDays(17), //17
                FirstName = "Людмила",
                Patronymic = "Семеновна",
                LastName = "Довгополая",
                Gender = Gender.All,
                PhoneNumber = "+380975362141",
            });
            context.SaveChanges();
            context.Calls.Add(new Call
            {
                CallId = Guid.Empty,
                PersonId = context.Persons.First().PersonId,
                CallDate = DateTime.Now.AddDays(-1),
                OrderCost = 0,
                CallReport = "Звонок прошел хорошо"
            });
            context.Calls.Add(new Call
            {
                CallId = Guid.Empty,
                PersonId = context.Persons.First().PersonId,
                CallDate = DateTime.Now.AddDays(-7),
                OrderCost = 0,
                CallReport = "Опреатор вне зоны доступа"
            });
            context.Calls.Add(new Call
            {
                CallId = Guid.Empty,
                PersonId = context.Persons.First().PersonId,
                CallDate = DateTime.Now.AddDays(-5),
                OrderCost = 199.55,
                CallReport = "Был сделан заказ"
            });
            context.Calls.Add(new Call
            {
                CallId = Guid.Empty,
                PersonId = context.Persons.Skip(1).First().PersonId,
                CallDate = DateTime.Now.AddDays(-15),
                OrderCost = 55.12,
                CallReport = "Часть заказа была с браком"
            });
            context.Calls.Add(new Call
            {
                CallId = Guid.Empty,
                PersonId = context.Persons.Skip(1).First().PersonId,
                CallDate = DateTime.Now.AddDays(-10),
                OrderCost = 0,
                CallReport = "С 12 до 16 - неудобное время"
            });
            context.Calls.Add(new Call
            {
                CallId = Guid.Empty,
                PersonId = context.Persons.Skip(1).First().PersonId,
                CallDate = DateTime.Now.AddDays(-8),
                OrderCost = 230.00,
                CallReport = "Повторный заказ 55"
            });
            context.Calls.Add(new Call
            {
                CallId = Guid.Empty,
                PersonId = context.Persons.Skip(2).First().PersonId,
                CallDate = DateTime.Now.AddDays(-16),
                OrderCost = 0,
                CallReport = "Отмена повторного заказа"
            });
            context.Calls.Add(new Call
            {
                CallId = Guid.Empty,
                PersonId = context.Persons.Skip(2).First().PersonId,
                CallDate = DateTime.Now.AddDays(-20),
                OrderCost = 330.85,
                CallReport = "Заказ из новой коллекции"
            });
            context.Calls.Add(new Call
            {
                CallId = Guid.Empty,
                PersonId = context.Persons.Skip(2).First().PersonId,
                CallDate = DateTime.Now.AddDays(-18),
                OrderCost = 5,
                CallReport = "Уточнение заказа и замена товара"
            });
            context.Calls.Add(new Call
            {
                CallId = Guid.Empty,
                PersonId = context.Persons.Skip(3).First().PersonId,
                CallDate = DateTime.Now.AddDays(-16),
                OrderCost = 0,
                CallReport = "Отмена повторного заказа"
            });
            context.Calls.Add(new Call
            {
                CallId = Guid.Empty,
                PersonId = context.Persons.Skip(3).First().PersonId,
                CallDate = DateTime.Now.AddDays(-20),
                OrderCost = 330.85,
                CallReport = "Заказ из новой коллекции"
            });
            context.Calls.Add(new Call
            {
                CallId = Guid.Empty,
                PersonId = context.Persons.Skip(3).First().PersonId,
                CallDate = DateTime.Now.AddDays(-18),
                OrderCost = 5,
                CallReport = "Уточнение заказа и замена товара"
            });
            context.SaveChanges();
        }
    }
}
