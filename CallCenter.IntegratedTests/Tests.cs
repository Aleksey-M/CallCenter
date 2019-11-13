using CallCenter.Back.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CallCenter
{
    public class UnitTests
    {
        private static DataBaseContext CreateCleareContext()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().Build();

            var builder = new DbContextOptionsBuilder<DataBaseContext>();
            builder.UseSqlite("Data Source=CallCenter.db;");

            var context = new DataBaseContext(builder.Options);
            context.Database.EnsureCreated();

            context.Calls.RemoveRange(context.Calls);
            context.Persons.RemoveRange(context.Persons);
            context.SaveChanges();

            return context;

            //var serviceProvider = new ServiceCollection()
            //    .AddEntityFrameworkInMemoryDatabase()
            //    .BuildServiceProvider();
            //var b = new DbContextOptionsBuilder<DataBaseContext>();
            //b.UseInMemoryDatabase("VirtualDB").UseInternalServiceProvider(serviceProvider);
            //var context = new DataBaseContext(b.Options);
            //context.Database.EnsureCreated();
            //return context;

            /* var serviceProvider = new ServiceCollection()
                 .AddEntityFrameworkSqlServer()
                 .BuildServiceProvider();
             var b = new DbContextOptionsBuilder<DataBaseContext>();
             b.UseSqlServer("Server=.\\SQLEXPRESS;AttachDbFilename={AppDomain.CurrentDomain.BaseDirectory}\\CallCenterBase.mdf;Database=CallCenterDb; Trusted_Connection=Yes;").UseInternalServiceProvider(serviceProvider);
             //$"Data Source=(localdb)\v11.0;AttachDbFileName={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CallCenterBase.mdf")};Integrated Security=true;
             //Server=ANTARES\\SQLEXPRESS;Database=CallCenter;Trusted_Connection=True;
             //Server=.\\SQLExpress;AttachDbFilename={AppDomain.CurrentDomain.BaseDirectory}\\CallCenterBase.mdf;Database=CallCenterDb; Trusted_Connection=Yes;


             var context = new DataBaseContext(b.Options);
             context.Database.EnsureCreated();
             context.Calls.RemoveRange(context.Calls);
             context.Persons.RemoveRange(context.Persons);
             context.SaveChanges();
             return context;
             */
        }

        [Fact]
        public async Task Persons_GetPersonsList_ShouldReturnList()
        {
            var context = CreateCleareContext();
            var pList = TestData.Get3TestPersons();
            context.Persons.AddRange(pList);
            await context.SaveChangesAsync();

            var list = await context.GetPersonsListAsync(PersonsFilterFields.Default);

            Assert.NotNull(list);
            Assert.Equal(3, list.Count);
            Assert.Contains(list, p => p.LastName == pList[0].LastName);
            Assert.Contains(list, p => p.LastName == pList[1].LastName);
            Assert.Contains(list, p => p.LastName == pList[2].LastName);
        }

        [Fact]
        public async Task Persons_GetPersonsCount_ShouldNotThrowAnyExceptions()
        {
            var context = CreateCleareContext();
            int count = await context.GetPersonsCountAsync(PersonsFilterFields.Default);

            Assert.True(count >= 0);
        }

        [Fact]
        public async Task Persons_SavePerson_ShouldIncreasePersonsCount()
        {
            var context = CreateCleareContext();

            var pList = TestData.Get3TestPersons();
            int count1 = await context.GetPersonsCountAsync(PersonsFilterFields.Default);

            await context.AddPersonAsync(pList[0]);
            int count2 = await context.GetPersonsCountAsync(PersonsFilterFields.Default);

            Assert.Equal(count1 + 1, count2);
        }

        [Fact]
        public async Task Persons_GetPerson_ShouldReturnSavedPerson()
        {
            var context = CreateCleareContext();
            var pList = TestData.Get3TestPersons();

            var savedId = await context.AddPersonAsync(pList[0]);
            var savedPerson = await context.GetPerson(savedId);

            Assert.Equal(pList[0], savedPerson);
        }

        [Fact]
        public async Task Persons_UpdatePerson_ShouldReturnUpdatedPerson()
        {
            var context = CreateCleareContext();
            var pList = TestData.Get3TestPersons();

            Guid pId = await context.AddPersonAsync(pList[0]);
            var pers = await context.GetPerson(pId);
            pers.FirstName = "NewFirstName";
            pers.LastName = "NewLastName";
            pers.Patronymic = "NewPatronymic";
            DateTime newBirthDate = DateTime.Now.AddYears(-20);
            pers.BirthDate = newBirthDate;
            pers.Gender = Gender.All;
            pers.PhoneNumber = "00000000";
            await context.UpdatePersonAsync(pers);
            pers = await context.GetPerson(pId);

            Assert.Equal("NewFirstName", pers.FirstName);
            Assert.Equal("NewLastName", pers.LastName);
            Assert.Equal("NewPatronymic", pers.Patronymic);
            Assert.Equal(newBirthDate, pers.BirthDate);
            Assert.Equal(Gender.All, pers.Gender);
            Assert.Equal("00000000", pers.PhoneNumber);
        }

        [Fact]
        public async Task Persons_DeletePerson_ShouldDeletePerson()
        {
            var context = CreateCleareContext();
            var pList = TestData.Get3TestPersons();

            Guid pId = await context.AddPersonAsync(pList[0]);
            var pers = await context.GetPerson(pId);
            await context.DeletePersonAsync(pers.PersonId);
            pers = await context.GetPerson(pers.PersonId);

            Assert.Null(pers);
        }

        [Fact]
        public async Task Calls_GetCalls_ShouldReturnEmptyList()
        {
            var context = CreateCleareContext();
            Guid pId = await context.AddPersonAsync(TestData.Get3TestPersons()[0]);

            var calls = await context.GetCalls(pId);

            Assert.Empty(calls);
        }

        [Fact]
        public async Task Calls_AddCall_ShouldNotThrowAnyAxception()
        {
            var context = CreateCleareContext();
            var callsList = TestData.Get9TestCalls();
            Guid pId = await context.AddPersonAsync(TestData.Get3TestPersons()[0]);

            await context.AddCallAsync(callsList[0], pId);
        }

        [Fact]
        public async Task Calls_GetCalls_ShouldReturnOneCall()
        {
            var context = CreateCleareContext();
            var callsList = TestData.Get9TestCalls();
            Guid pId = await context.AddPersonAsync(TestData.Get3TestPersons()[0]);

            await context.AddCallAsync(callsList[0], pId);
            var sCalls = await context.GetCalls(pId);

            Assert.Single(sCalls);
        }

        [Fact]
        public async Task Calls_UpdateCall_ShouldUpdateCallFields()
        {
            var context = CreateCleareContext();
            var callsList = TestData.Get9TestCalls();
            Guid pId = await context.AddPersonAsync(TestData.Get3TestPersons()[0]);
            await context.AddCallAsync(callsList[0], pId);
            var sCall = (await context.GetCalls(pId))[0];

            sCall.CallDate = DateTime.Now.AddDays(-1);
            sCall.CallReport = "call report";
            sCall.OrderCost = 999.99;
            await context.UpdateCallAsync(sCall);
            var sCall2 = (await context.GetCalls(pId))[0];

            Assert.Equal(sCall, sCall2);
            Assert.Equal(sCall2.CallDate, sCall.CallDate);
            Assert.Equal(sCall2.CallReport, sCall.CallReport);
            Assert.Equal(sCall2.OrderCost, sCall.OrderCost);
        }

        [Fact]
        public async Task Calls_DeleteCall_ShouldDeleteCall()
        {
            var context = CreateCleareContext();
            var callsList = TestData.Get9TestCalls();
            Guid pId = await context.AddPersonAsync(TestData.Get3TestPersons()[0]);

            await context.AddCallAsync(callsList[0], pId);
            var sCalls = await context.GetCalls(pId);
            await context.DeleteCallAsync(sCalls[0].CallId);

            Assert.True(context.Calls.FirstOrDefault(c => c.CallId == sCalls[0].PersonId) == null);
        }

        private async Task<List<Person>> Get3SavedTestPersons(DataBaseContext context)
        {
            var pList = TestData.Get3TestPersons(false, false);
            var pCalls = TestData.Get9TestCalls();
            foreach (var p in pList)
            {
                p.PersonId = await context.AddPersonAsync(p);
            }

            for (int i = 0; i < 3; i++)
            {
                await context.AddCallAsync(pCalls[3 * i], pList[i].PersonId);
                await context.AddCallAsync(pCalls[3 * i + 1], pList[i].PersonId);
                await context.AddCallAsync(pCalls[3 * i + 2], pList[i].PersonId);
            }
            return pList;
        }

        [Fact]
        public async Task Filtering_WithEmptyFilters_ShouldReturn3Persons()
        {
            var context = CreateCleareContext();
            await Get3SavedTestPersons(context);
            // вывод без фильтров
            var resList = await context.GetPersonsAsync(PersonsFilterFields.Default);

            Assert.True(resList.Count == 3, "Не все элементы прошли фильтр без условий");
        }

        [Fact]
        public async Task Filtering_WithNameFilter_ShouldReturn1PersonFor3Fields()
        {
            var context = CreateCleareContext();
            var persons = await Get3SavedTestPersons(context);
            //Фильтрация по имени(имени, фамилии или отчеству)
            var filter = new PersonsFilterFields() { NameFilter = persons[0].FirstName };
            var resList = await context.GetPersonsAsync(filter);
            Assert.Single(resList);
            filter.NameFilter = persons[0].LastName;
            resList = await context.GetPersonsAsync(filter);
            Assert.Single(resList);
            filter.NameFilter = persons[1].Patronymic;
            resList = await context.GetPersonsAsync(filter);
            Assert.Single(resList);
        }

        [Fact]
        public async Task Filtering_WithLastCallDateFilter_ShouldReturn2Persons()
        {
            var context = CreateCleareContext();
            await Get3SavedTestPersons(context);
            // Фильтр по дате последнего звонка (отбор тех, кому не звонили N-е количество дней)
            var resList = await context.GetPersonsAsync(new PersonsFilterFields() { MinDaysAfterLastCall = 7 });
            Assert.Equal(2, resList.Count);
            Assert.True(resList.Find(p => p.FirstName == "Евгений") != null, "Неправильный результат фильтрации по дате последнего звонка");
            Assert.True(resList.Find(p => p.FirstName == "Ольга") != null, "Неправильный результат фильтрации по дате последнего звонка");
        }

        [Fact]
        public async Task Filtering_WithGenderFilter_ShouldReturnOnePerson()
        {
            var context = CreateCleareContext();
            await Get3SavedTestPersons(context);
            // Фильтрация по полу
            var resList = await context.GetPersonsAsync(new PersonsFilterFields() { Gender = Gender.Male });
            Assert.True(resList.Count == 1, "Не работает фильтр по полу");
            Assert.True(resList.Find(p => p.FirstName == "Евгений") != null, "Неправильный результат фильтрации по полу");
        }

        [Fact]
        public async Task Filtering_WithAgeFilter_ShouldReturnOnePerson()
        {
            var context = CreateCleareContext();
            await Get3SavedTestPersons(context);
            // фильтрация по возрасту
            var resList = await context.GetPersonsAsync(new PersonsFilterFields() { MaxAge = 20, MinAge = 15 });
            Assert.True(resList.Count == 1, "Не сработал фильтр по возрасту");
            Assert.True(resList.Find(p => p.FirstName == "Евгений") != null, "Неправильный результат фильтрации по возрасту");
        }

        [Fact]
        public async Task Paging_ShouldReturnSecondPageWithOnePerson()
        {
            var context = CreateCleareContext();
            await Get3SavedTestPersons(context);
            // проверка пейджинга           
            var resList = await context.GetPersonsAsync(new PersonsFilterFields() { PageSize = 2, PageNo = 2 });
            Assert.True(resList.Count == 1, "Не работает разбиение на страницы");
        }

    }
}
