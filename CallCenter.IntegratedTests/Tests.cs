using CallCenter.Back.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace CallCenter.IntegratedTests
{    
    public class Tests
    {
        private static DataBaseContext CreateCleareInMemoryContext()
        {
            /*
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();
            var b = new DbContextOptionsBuilder<DataBaseContext>();
            b.UseInMemoryDatabase("VirtualDB").UseInternalServiceProvider(serviceProvider);
            var context = new DataBaseContext(b.Options);
            context.Database.EnsureCreated();
            return context;
            */
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlServer()
                .BuildServiceProvider();
            var b = new DbContextOptionsBuilder<DataBaseContext>();
            b.UseSqlServer("Server=.\\SQLExpress;AttachDbFilename={AppDomain.CurrentDomain.BaseDirectory}\\CallCenterBase.mdf;Database=CallCenterDb; Trusted_Connection=Yes;").UseInternalServiceProvider(serviceProvider);
            //$"Data Source=(localdb)\v11.0;AttachDbFileName={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CallCenterBase.mdf")};Integrated Security=true;
            //Server=ANTARES\\SQLEXPRESS;Database=CallCenter;Trusted_Connection=True;
            //Server=.\\SQLExpress;AttachDbFilename={AppDomain.CurrentDomain.BaseDirectory}\\CallCenterBase.mdf;Database=CallCenterDb; Trusted_Connection=Yes;
            var context = new DataBaseContext(b.Options);
            context.Database.EnsureCreated();
            context.Calls.RemoveRange(context.Calls);
            context.Persons.RemoveRange(context.Persons);
            context.SaveChanges();
            return context;
        }
        
        [Fact]
        public void Persons_GetPersonsList_ShouldReturnList()
        {
            var context = CreateCleareInMemoryContext();
            var pList = TestData.Get3TestPersons();
            pList.ForEach(p => context.Persons.Add(p));
            context.SaveChanges();

            var list = context.GetPersonsListAsync(PersonsFilterFields.Default).Result;

            Assert.NotNull(list);
            Assert.Equal(3, list.Count);
            Assert.Contains(list, p => p.LastName == pList[0].LastName);
            Assert.Contains(list, p => p.LastName == pList[1].LastName);
            Assert.Contains(list, p => p.LastName == pList[2].LastName);
        }

        [Fact]
        public void Persons_GetPersonsCount_ShouldNotThrowAnyExceptions()
        {
            var context = CreateCleareInMemoryContext();
            int count = context.GetPersonsCountAsync(PersonsFilterFields.Default).Result;

            Assert.True(count >= 0);
        }

        [Fact]
        public void Persons_SavePerson_ShouldIncreasePersonsCount()
        {
            var context = CreateCleareInMemoryContext();

            var pList = TestData.Get3TestPersons();
            int count1 = context.GetPersonsCountAsync(PersonsFilterFields.Default).Result;

            context.AddPersonAsync(pList[0]).Wait();
            int count2 = context.GetPersonsCountAsync(PersonsFilterFields.Default).Result;

            Assert.Equal(count1 + 1, count2);
        }

        [Fact]
        public void Persons_GetPerson_ShouldReturnSavedPerson()
        {
            var context = CreateCleareInMemoryContext();
            var pList = TestData.Get3TestPersons();

            var savedId = context.AddPersonAsync(pList[0]).Result;
            //Guid pId = context.Persons.FirstAsync(p => pList[0].Equals(p)).Result.PersonId;
            var savedPerson = context.GetPerson(savedId);           
            
            Assert.Equal(pList[0], savedPerson);           
        }

        [Fact]
        public void Persons_UpdatePerson_ShouldReturnUpdatedPerson()
        {
            var context = CreateCleareInMemoryContext();
            var pList = TestData.Get3TestPersons();

            Guid pId = context.AddPersonAsync(pList[0]).Result;
            var pers = context.GetPerson(pId);
            pers.FirstName = "NewFirstName";
            pers.LastName = "NewLastName";
            pers.Patronymic = "NewPatronymic";
            DateTime newBirthDate = DateTime.Now.AddYears(-20);
            pers.BirthDate = newBirthDate;
            pers.Gender = Gender.All;
            pers.PhoneNumber = "00000000";
            context.UpdatePersonAsync(pers).Wait();
            pers = context.GetPerson(pId);

            Assert.Equal("NewFirstName", pers.FirstName);
            Assert.Equal("NewLastName", pers.LastName);
            Assert.Equal("NewPatronymic", pers.Patronymic);
            Assert.Equal(newBirthDate, pers.BirthDate);
            Assert.Equal(Gender.All, pers.Gender);
            Assert.Equal("00000000", pers.PhoneNumber);
        }

        [Fact]
        public void Persons_DeletePerson_ShouldDeletePerson()
        {
            var context = CreateCleareInMemoryContext();
            var pList = TestData.Get3TestPersons();

            Guid pId = context.AddPersonAsync(pList[0]).Result;
            var pers = context.GetPerson(pId);
            context.DeletePersonAsync(pers.PersonId).Wait();
            pers = context.GetPerson(pers.PersonId);

            Assert.Null(pers);
        }

        [Fact]
        public void Calls_GetCalls_ShouldReturnEmptyList()
        {
            var context = CreateCleareInMemoryContext();
            Guid pId = context.AddPersonAsync(TestData.Get3TestPersons()[0]).Result;

            var calls = context.GetCalls(pId);

            Assert.Empty(calls);
        }

        [Fact]
        public void Calls_AddCall_ShouldNotThrowAnyAxception()
        {
            var context = CreateCleareInMemoryContext();
            var callsList = TestData.Get9TestCalls();
            Guid pId = context.AddPersonAsync(TestData.Get3TestPersons()[0]).Result;

            context.AddCallAsync(callsList[0], pId).Wait();
        }

        [Fact]
        public void Calls_GetCalls_ShouldReturnOneCall()
        {
            var context = CreateCleareInMemoryContext();
            var callsList = TestData.Get9TestCalls();
            Guid pId = context.AddPersonAsync(TestData.Get3TestPersons()[0]).Result;

            context.AddCallAsync(callsList[0], pId).Wait();
            var sCalls = context.GetCalls(pId);

            Assert.Single(sCalls);
        }

        [Fact]
        public void Calls_UpdateCall_ShouldUpdateCallFields()
        {
            var context = CreateCleareInMemoryContext();
            var callsList = TestData.Get9TestCalls();
            Guid pId = context.AddPersonAsync(TestData.Get3TestPersons()[0]).Result;
            context.AddCallAsync(callsList[0], pId).Wait();
            var sCall = context.GetCalls(pId)[0];

            sCall.CallDate = DateTime.Now.AddDays(-1);
            sCall.CallReport = "call report";
            sCall.OrderCost = 999.99;
            context.UpdateCallAsync(sCall).Wait();
            var sCall2 = context.GetCalls(pId)[0];

            Assert.Equal(sCall, sCall2);
            Assert.Equal(sCall2.CallDate, sCall.CallDate);
            Assert.Equal(sCall2.CallReport, sCall.CallReport);
            Assert.Equal(sCall2.OrderCost, sCall.OrderCost);
        }

        [Fact]
        public void Calls_DeleteCall_ShouldDeleteCall()
        {
            var context = CreateCleareInMemoryContext();
            var callsList = TestData.Get9TestCalls();
            Guid pId = context.AddPersonAsync(TestData.Get3TestPersons()[0]).Result;

            context.AddCallAsync(callsList[0], pId).Wait();
            var sCalls = context.GetCalls(pId);
            context.DeleteCallAsync(sCalls[0].CallId).Wait();

            Assert.True(context.Calls.FirstOrDefault(c=>c.CallId == sCalls[0].PersonId) == null);
        }
        
        private List<Person> Get3SavedTestPersons(DataBaseContext context)
        {
            var pList = TestData.Get3TestPersons(false, false);
            var pCalls = TestData.Get9TestCalls();
            foreach (var p in pList)
            {
                p.PersonId = context.AddPersonAsync(p).Result;
            }
            
            for (int i = 0; i < 3; i++)
            {
                context.AddCallAsync(pCalls[3 * i], pList[i].PersonId).Wait();
                context.AddCallAsync(pCalls[3 * i + 1], pList[i].PersonId).Wait();
                context.AddCallAsync(pCalls[3 * i + 2], pList[i].PersonId).Wait();
            }
            return pList;
        }

        [Fact]
        public void Filtering_WithEmptyFilters_ShouldReturn3Persons()
        {
            var context = CreateCleareInMemoryContext();
            Get3SavedTestPersons(context);
            // вывод без фильтров
            var resList = context.GetPersonsAsync(PersonsFilterFields.Default).Result;

            Assert.True(resList.Count == 3, "Не все элементы прошли фильтр без условий");
        }

        [Fact]
        public void Filtering_WithNameFilter_ShouldReturn1PersonFor3Fields()
        {
            var context = CreateCleareInMemoryContext();
            var persons = Get3SavedTestPersons(context);
            //Фильтрация по имени(имени, фамилии или отчеству)
            var filter = new PersonsFilterFields() { NameFilter = persons[0].FirstName };
            var resList = context.GetPersonsAsync(filter).Result;
            Assert.Single(resList);
            filter.NameFilter = persons[0].LastName;
            resList = context.GetPersonsAsync(filter).Result;
            Assert.Single(resList);
            filter.NameFilter = persons[1].Patronymic;
            resList = context.GetPersonsAsync(filter).Result;
            Assert.Single(resList);
        }

        [Fact]
        public void Filtering_WithLastCallDateFilter_ShouldReturn2Persons()
        {
            var context = CreateCleareInMemoryContext();
            Get3SavedTestPersons(context);
            // Фильтр по дате последнего звонка (отбор тех, кому не звонили N-е количество дней)
            var resList = context.GetPersonsAsync(new PersonsFilterFields() { MinDaysAfterLastCall = 7 }).Result;
            Assert.Equal(2, resList.Count);
            Assert.True(resList.Find(p => p.FirstName == "Евгений") != null, "Неправильный результат фильтрации по дате последнего звонка");
            Assert.True(resList.Find(p => p.FirstName == "Ольга") != null, "Неправильный результат фильтрации по дате последнего звонка");
        }

        [Fact]
        public void Filtering_WithGenderFilter_ShouldReturnOnePerson()
        {
            var context = CreateCleareInMemoryContext();
            Get3SavedTestPersons(context);
            // Фильтрация по полу
            var resList = context.GetPersonsAsync(new PersonsFilterFields() { Gender = Gender.Male }).Result;
            Assert.True(resList.Count == 1, "Не работает фильтр по полу");
            Assert.True(resList.Find(p => p.FirstName == "Евгений") != null, "Неправильный результат фильтрации по полу");
        }

        [Fact]
        public void Filtering_WithAgeFilter_ShouldReturnOnePerson()
        {
            var context = CreateCleareInMemoryContext();
            Get3SavedTestPersons(context);
            // фильтрация по возрасту
            var resList = context.GetPersonsAsync(new PersonsFilterFields() { MaxAge = 20, MinAge = 15 }).Result;
            Assert.True(resList.Count == 1, "Не сработал фильтр по возрасту");
            Assert.True(resList.Find(p => p.FirstName == "Евгений") != null, "Неправильный результат фильтрации по возрасту");
        }

        [Fact]
        public void Paging_ShouldReturnSecondPageWithOnePerson()
        {
            var context = CreateCleareInMemoryContext();
            Get3SavedTestPersons(context);
            // проверка пейджинга           
            var resList = context.GetPersonsAsync(new PersonsFilterFields() { PageSize = 2, PageNo = 2 }).Result;
            Assert.True(resList.Count == 1, "Не работает разбиение на страницы");
        }
   
    }
}
