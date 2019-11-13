using CallCenter.Back.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CallCenter
{
    public class WebApiTests
    {
#warning Перед запуском тестов должно быть запущено Web-приложение и обновлен номер порта. Используется рабочая база данных        
        private string BaseAddress { get { return "http://localhost:55626/"; } }
        private HttpClient GetClient()
        {
            var client = new HttpClient()
            {
                BaseAddress = new Uri(BaseAddress)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
        private async Task DeletePersonFromBaseIfExists(string personName)
        {
            using var client = GetClient();
            using var response = await client.GetAsync($"api/persons?NameFilter={personName}");
            if (response.IsSuccessStatusCode)
            {
                var pList = await response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>();
                foreach (var pers in pList)
                {
                    var r = await client.DeleteAsync($"api/persons/{pers.Id}");
                }
            }
        }

        private async Task<Person> AddPersonAndGetItWithId()
        {
            var person = new Person()
            {
                FirstName = "Арнольд_" + Guid.NewGuid().ToString().Substring(0, 8),
                LastName = "Шварценеггер_" + Guid.NewGuid().ToString().Substring(0, 8),
                Patronymic = "Георгиевич_" + Guid.NewGuid().ToString().Substring(0, 8),
                Gender = Gender.Male,
                BirthDate = DateTime.Now.AddYears((DateTime.Now.Second * (-1)) - 18),
                PhoneNumber = "0984562121"
            };
            await DeletePersonFromBaseIfExists(person.FirstName);

            using (var client = GetClient())
            {
                using (var response = await client.PostAsJsonAsync("api/persons", person))
                {
                    if (!response.IsSuccessStatusCode) return null;
                }
                using (var response = await client.GetAsync($"api/persons?NameFilter={person.FirstName}"))
                {
                    if (!response.IsSuccessStatusCode) return null;
                    var list = await response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>();
                    if (list == null) return null;
                    person.PersonId = list.First().Id;
                }
            }
            return person;
        }

        [Fact]
        public async Task WebApi_GetCount()
        {
            using var client = GetClient();
            using var response = await client.GetAsync("api/persons/count");
            Assert.True(response.IsSuccessStatusCode, $"Неправильный статусный код при запросе количества записей: {response.StatusCode}");
            int recCount = await response.Content.ReadAsAsync<int>();
            Assert.True(recCount >= 0, "Отрицательное значение Count");
        }

        [Fact]
        public async Task WebApi_AddPerson()
        {
            using var client = GetClient();
            int pCount, newCount;
            using (var response = await client.GetAsync("api/persons/count"))
            {
                Assert.True(response.IsSuccessStatusCode, $"Неправильный статусный код при запросе количества записей: {response.StatusCode}");
                pCount = await response.Content.ReadAsAsync<int>();
            }

            var person = new Person()
            {
                PersonId = Guid.Empty,
                BirthDate = DateTime.Parse("20.05.1990", CultureInfo.GetCultureInfo("ru-RU")),
                FirstName = "Евгения",
                Patronymic = "Федоровна",
                LastName = "Полкина",
                Gender = Gender.Female,
                PhoneNumber = "+380997586636"
            };
            using (var response = await client.PostAsJsonAsync("api/persons", person))
            {
                Assert.True(response.IsSuccessStatusCode, $"Неправильный статусный код: {response.StatusCode}");
            }

            using (var response = await client.GetAsync("api/persons/count"))
            {
                Assert.True(response.IsSuccessStatusCode, $"Неправильный статусный код при запросе количества записей: {response.StatusCode}");
                newCount = await response.Content.ReadAsAsync<int>();
            }
            Assert.True(pCount + 1 == newCount, $"Запись не добавлена: До добавления записей - {pCount},  после добавления - {newCount}");
        }

        [Fact]
        public async Task WebApi_GetLastPageWithOnePerson()
        {
            using var client = GetClient();
            int pCount;
            using (var response = await client.GetAsync("api/persons/count"))
            {
                Assert.True(response.IsSuccessStatusCode, $"Неправильный статусный код при запросе количества записей: {response.StatusCode}");
                pCount = await response.Content.ReadAsAsync<int>();
            }

            Assert.True(pCount != 0, "Для выполнения теста недостаточно записей в базе");

            using (var response = await client.GetAsync($"api/persons?PageSize=1&PageNo=/{pCount}"))
            {
                Assert.True(response.IsSuccessStatusCode, $"Неправильный код ответа при запросе страницы - {response.StatusCode}");
                var page = await response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>();
                Assert.True(page != null, "Пустй ответ при запросе страницы");
                Assert.True(page.Count() == 1, $"Страница не содержит ожидаемого количества данных: {page.Count()}");
            }
        }

        [Fact]
        public async Task WebApi_GetFirstPageWithThreePersons()
        {
            using var client = GetClient();
            int pCount;
            using (var response = await client.GetAsync("api/persons/count"))
            {
                Assert.True(response.IsSuccessStatusCode, $"Неправильный статусный код при запросе количества записей: {response.StatusCode}");
                pCount = await response.Content.ReadAsAsync<int>();
            }

            Assert.True(pCount >= 3, "Для выполнения теста недостаточно записей в базе");

            using (var response = await client.GetAsync("api/persons?PageSize=3&PageNo=1"))
            {
                Assert.True(response.IsSuccessStatusCode, $"Неправильный код ответа при запросе страницы - {response.StatusCode}");
                var page = await response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>();
                Assert.True(page != null, "Пустй ответ при запросе страницы");
                Assert.True(page.Count() == 3, $"Страница не содержит ожидаемого количества данных: {page.Count()}");
            }
        }

        [Fact]
        public async Task WebApi_GetPersonById()
        {
            using var client = GetClient();
            var person = new Person() { FirstName = $"Name_{Guid.NewGuid().ToString().Substring(0, 8)}", PhoneNumber = "test number" };
            using (var response = await client.PostAsJsonAsync("api/persons", person))
            {
                Assert.True(response.IsSuccessStatusCode, $"Неправильный статусный код при добавлении записи: {response.StatusCode}");
            }

            using (var response = await client.GetAsync($"api/persons?NameFilter={Uri.EscapeDataString(person.FirstName)}"))
            {
                Assert.True(response.IsSuccessStatusCode, $"Неправильный код ответа при запросе страницы - {response.StatusCode}");
                var page = await response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>();
                Assert.True(page != null, "Пустой ответ при запросе страницы");
                Assert.True(page.Count() == 1, $"Страница не содержит ожидаемого количества данных: {page.Count()}");
                person.PersonId = page.First().Id;
            }

            using (var response = await client.GetAsync($"api/persons/{Uri.EscapeDataString(person.PersonId.ToString())}"))
            {
                Assert.True(response.IsSuccessStatusCode, $"Неправильный код ответа при запросе страницы - {response.StatusCode}");
                var pers = await response.Content.ReadAsAsync<Person>();
                Assert.True(pers != null, $"Запись не найдена по id - {person.PersonId}");
            }
        }

        [Fact]
        public async Task WebApi_GetPageFilteredByName()
        {
            using var client = GetClient();
            var person = new Person()
            {
                FirstName = $"Name_{Guid.NewGuid().ToString().Substring(0, 8)}",
                LastName = $"Name_{Guid.NewGuid().ToString().Substring(0, 8)}",
                Patronymic = $"Name_{Guid.NewGuid().ToString().Substring(0, 8)}",
                PhoneNumber = "phone number"
            };
            using (var response = await client.PostAsJsonAsync("api/persons", person))
            {
                Assert.True(response.IsSuccessStatusCode, $"Неправильный статусный код при добавлении записи: {response.StatusCode}");
            }

            using (var response = await client.GetAsync($"api/persons?NameFilter={person.FirstName}"))
            {
                Assert.True(response.IsSuccessStatusCode, $"Неправильный код ответа при запросе страницы - {response.StatusCode}");
                var page = await response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>();
                Assert.True(page != null, "Пустй ответ при запросе страницы");
                Assert.True(page.Count() == 1, $"Страница не содержит ожидаемого количества данных: {page.Count()}");
                Assert.True(person.FirstName == page.First().FirstName, "Получены некорректные данные (FirstName)");
            }

            using (var response = await client.GetAsync($"api/persons?NameFilter={person.LastName}"))
            {
                Assert.True(response.IsSuccessStatusCode, $"Неправильный код ответа при запросе страницы - {response.StatusCode}");
                var page = await response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>();
                Assert.True(page != null, "Пустй ответ при запросе страницы");
                Assert.True(page.Count() == 1, $"Страница не содержит ожидаемого количества данных: {page.Count()}");
                Assert.True(person.LastName == page.First().LastName, "Получены некорректные данные (LastName)");
            }

            using (var response = await client.GetAsync($"api/persons?NameFilter={person.Patronymic}"))
            {
                Assert.True(response.IsSuccessStatusCode, $"Неправильный код ответа при запросе страницы - {response.StatusCode}");
                var page = await response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>();
                Assert.True(page != null, "Пустй ответ при запросе страницы");
                Assert.True(page.Count() == 1, $"Страница не содержит ожидаемого количества данных: {page.Count()}");
                Assert.True(person.Patronymic == page.First().Patronymic, "Получены некорректные данные (Patronymic)");
            }
        }

        [Fact]
        public async Task WebApi_GetPageFilteredByGender()
        {
            using var client = GetClient();
            int allCount, maleCount, femaleCount, pCount;
            using (var response = await client.GetAsync($"api/persons?Gender={Gender.All}&pagesize=500"))
            {
                Assert.True(response.IsSuccessStatusCode, $"Код запроса отфильрованного списка (Gender.All) - {response.StatusCode}");
                var page = await response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>();
                allCount = page.Count();
                Assert.True(allCount > 0, "Недостаточно записей в базе для тестирования");
            }
            using (var response = await client.GetAsync($"api/persons?Gender={Gender.Female}&pagesize=500"))
            {
                Assert.True(response.IsSuccessStatusCode, $"Код запроса отфильрованного списка (Gender.Female) - {response.StatusCode}");
                var page = await response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>();
                femaleCount = page.Count();
            }
            using (var response = await client.GetAsync($"api/persons?Gender={Gender.Male}&pagesize=500"))
            {
                Assert.True(response.IsSuccessStatusCode, $"Код запроса отфильрованного списка (Gender.Male) - {response.StatusCode}");
                var page = await response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>();
                maleCount = page.Count();
            }
            using (var response = await client.GetAsync("api/persons/count"))
            {
                Assert.True(response.IsSuccessStatusCode, $"Неправильный статусный код при запросе количества записей: {response.StatusCode}");
                pCount = await response.Content.ReadAsAsync<int>();
            }
            Assert.True(allCount == pCount, $"Ошибка в определении пола. All:{allCount}; Records Count:{pCount};");
        }

        [Fact]
        public async Task WebApi_GetPageFilteredByMaxAge()
        {
            await DeletePersonFromBaseIfExists("MaxAgeTestUser");
            using var client = GetClient();
            var person = new Person() { FirstName = "MaxAgeTestUser", PhoneNumber = "444-444", BirthDate = DateTime.Now.AddYears(-1) };
            await DeletePersonFromBaseIfExists(person.FirstName);
            Thread.Sleep(1000);
            var res = await client.PostAsJsonAsync("api/persons", person);

            using (res = await client.GetAsync("api/persons?MaxAge=1"))
            {
                Assert.True(res.IsSuccessStatusCode, $"Код ответа не положительный- {res.StatusCode}");
                var pList = await res.Content.ReadAsAsync<IEnumerable<PersonsListItem>>();
                Assert.True(pList.Count() == 1, $"Страница не содержит ожидаемого количества данных: {pList.Count()}");
                Assert.True(person.FirstName == pList.First().FirstName, "Получены некорректные данные");
            }
        }

        [Fact]
        public async Task WebApi_GetPageFilteredByMinAge()
        {
            using var client = GetClient();
            var person = new Person() { FirstName = "MinAgeTestUser", PhoneNumber = "444-444", BirthDate = DateTime.Now.AddYears(-100) };
            await DeletePersonFromBaseIfExists(person.FirstName);

            using (var res = await client.PostAsJsonAsync("api/persons", person)) { }

            using (var res = await client.GetAsync("api/persons?MinAge=100&MaxAge=101"))
            {
                Assert.True(res.IsSuccessStatusCode, $"Код ответа не положительный - {res.StatusCode}");
                var pList = await res.Content.ReadAsAsync<IEnumerable<PersonsListItem>>();
                Assert.True(pList.Count() == 1, "Страница не содержит ожидаемого количества данных");
                Assert.True(person.FirstName == pList.First().FirstName, "Получены некорректные данные");
            }
        }

        [Fact]
        public async Task WebApi_UpdatePerson()
        {
            using var client = GetClient();
            var person = new Person()
            {
                FirstName = "UpdateName",
                LastName = "UpdateName2",
                Patronymic = "UpdatePatronymic",
                BirthDate = DateTime.Now.AddYears(-30),
                Gender = Gender.All,
                PhoneNumber = "1111111111"
            };

            await DeletePersonFromBaseIfExists(person.FirstName);
            using (var res = await client.PostAsJsonAsync("api/persons", person)) { }

            using (var res = await client.GetAsync($"api/persons?NameFilter={person.FirstName}"))
            {
                Assert.True(res.IsSuccessStatusCode, $"Код ответа не положительный - {res.StatusCode}");
                var pList = await res.Content.ReadAsAsync<IEnumerable<PersonsListItem>>();
                Assert.True(1 == pList.Count(), "Страница не содержит ожидаемого количества данных");
                Assert.True(person.FirstName == pList.First().FirstName, "Получены некорректные данные");
                person.PersonId = pList.First().Id;
            }

            person.FirstName = "NewName";
            person.LastName = "NewName2";
            person.Patronymic = "NewPatronymic";
            person.PhoneNumber = "2222222222";
            person.BirthDate = person.BirthDate?.AddYears(10);
            person.Gender = Gender.Male;
            await DeletePersonFromBaseIfExists(person.FirstName);
            using (var res = await client.PutAsJsonAsync($"api/persons", person)) { }

            Person personById, personByName;
            personByName = new Person();
            using (var res = await client.GetAsync($"api/persons/{person.PersonId}"))
            {
                personById = await res.Content.ReadAsAsync<Person>();
            }
            using (var res = await client.GetAsync($"api/persons?NameFilter={person.FirstName}"))
            {
                var list = await res.Content.ReadAsAsync<IEnumerable<PersonsListItem>>();
                Assert.True(1 == list.Count(), "Страница не содержит ожидаемого количества данных");
                personByName.PersonId = list.First().Id;
            }

            using (var res = await client.GetAsync($"api/persons/{personByName.PersonId}"))
            {
                personByName = await res.Content.ReadAsAsync<Person>();
            }

            Assert.True(person.FirstName == personById.FirstName, "Получены некорректные данные по Id");
            Assert.True(person.LastName == personById.LastName, "Получены некорректные данные по Id");
            Assert.True(person.Patronymic == personById.Patronymic, "Получены некорректные данные по Id");
            Assert.True(person.PhoneNumber == personById.PhoneNumber, "Получены некорректные данные по Id");
            Assert.True(person.BirthDate?.Date == personById.BirthDate?.Date, "Получены некорректные данные по Id");
            Assert.True(person.Gender == personById.Gender, "Получены некорректные данные по Id");

            Assert.True(person.PersonId == personByName.PersonId, "Получены некорректные данные по FirstName");
            Assert.True(person.FirstName == personByName.FirstName, "Получены некорректные данные по FirstName");
            Assert.True(person.LastName == personByName.LastName, "Получены некорректные данные по FirstName");
            Assert.True(person.Patronymic == personByName.Patronymic, "Получены некорректные данные по FirstName");
            Assert.True(person.PhoneNumber == personByName.PhoneNumber, "Получены некорректные данные по FirstName");
            Assert.True(person.Gender == personByName.Gender, "Получены некорректные данные по FirstName");
            Assert.True(person.BirthDate?.Date == personByName.BirthDate?.Date, "Получены некорректные данные по FirstName");
        }

        [Fact]
        public async Task WebApi_DeletePerson()
        {
            using var client = GetClient();
            var person = new Person() { FirstName = "DeletingTest", PhoneNumber = "77777777" };

            using (var res = await client.PostAsJsonAsync("api/persons", person)) { }

            using (var res = await client.GetAsync($"api/persons?NameFilter={person.FirstName}"))
            {
                Assert.True(res.IsSuccessStatusCode, $"Код ответа не положительный - {res.StatusCode}");
                var resList = await res.Content.ReadAsAsync<IEnumerable<PersonsListItem>>();
                Assert.True(1 == resList.Count(), "Страница не содержит ожидаемого количества данных");
                person.PersonId = resList.First().Id;
            }

            using (var res = await client.DeleteAsync($"api/persons/{person.PersonId}"))
            {
                Assert.True(res.IsSuccessStatusCode, $"Код ответа не положительный - {res.StatusCode}");
            }
            using (var res = await client.GetAsync($"api/persons/{person.PersonId}"))
            {
                Assert.True(res.StatusCode == HttpStatusCode.NotFound, "Запись не удалена");
            }
        }

        [Fact]
        public async Task WebApi_AddCall()
        {
            using var client = GetClient();
            var person = await AddPersonAndGetItWithId();
            var call = new Call()
            {
                CallDate = DateTime.Now.AddDays(-10),
                CallReport = "Звонок прошел на удивление хорошо",
                OrderCost = 489.54
            };
            using (var resp = await client.PostAsJsonAsync($"api/persons/{person.PersonId}/calls", call))
            {
                Assert.True(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
            }
            Guid fcId;
            using (var resp = await client.GetAsync($"api/persons/{person.PersonId}/calls"))
            {
                Assert.True(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
                var c = await resp.Content.ReadAsAsync<IEnumerable<Call>>();
                Assert.True(c != null, "После обновления записи не удалось загрузить данные");
                Assert.True(1 == c.Count(), $"Неожиданное количество отчетов после добавления: {c.Count()}");

                Assert.True(call.CallDate.Date == c.First().CallDate.Date, "Получены некорректные данные (CallDate)");
                Assert.True(call.CallReport == c.First().CallReport, "Получены некорректные данные (CallReport)");
                Assert.True(call.OrderCost == c.First().OrderCost, "Получены некорректные данные (OrderCost)");

                fcId = c.First().CallId;
            }

            call = new Call()
            {
                CallDate = DateTime.Now.AddDays(-5),
                CallReport = "Звонок прошел нормально",
                OrderCost = 0
            };
            using (var resp = await client.PostAsJsonAsync($"api/persons/{person.PersonId}/calls", call))
            {
                Assert.True(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
            }
            using (var resp = await client.GetAsync($"api/persons/{person.PersonId}/calls"))
            {
                Assert.True(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
                var c = await resp.Content.ReadAsAsync<IEnumerable<Call>>();
                Assert.True(c != null, "После обновления записи не удалось загрузить данные");
                Assert.True(2 == c.Count(), $"Неожиданное количество отчетов после добавления: {c.Count()}");

                Assert.True(call.CallDate.Date == c.FirstOrDefault(cl => cl.CallId != fcId).CallDate.Date, "Получены некорректные данные (CallDate)");
                Assert.True(call.CallReport == c.FirstOrDefault(cl => cl.CallId != fcId).CallReport, "Получены некорректные данные (CallReport)");
                Assert.True(call.OrderCost == c.FirstOrDefault(cl => cl.CallId != fcId).OrderCost, "Получены некорректные данные (OrderCost)");
            }
        }

        [Fact]
        public async Task WebApi_UpdateCall()
        {
            using var client = GetClient();
            var person = await AddPersonAndGetItWithId();
            var call = new Call()
            {
                CallDate = DateTime.Now.AddDays(-7),
                CallReport = "Предложение по новой коллекции понравилось клиенту, но он решил заказать предыдущий товар",
                OrderCost = 229.99
            };
            using (var resp = await client.PostAsJsonAsync($"api/persons/{person.PersonId}/calls", call))
            {
                Assert.True(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
            }
            using (var resp = await client.GetAsync($"api/persons/{person.PersonId}/calls"))
            {
                Assert.True(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
                var calls = await resp.Content.ReadAsAsync<IEnumerable<Call>>();
                Assert.True(calls != null, "После обновления записи не удалось загрузить данные");
                Assert.True(1 == calls.Count(), $"Неожиданное количество отчетов после добавления: {calls.Count()}");

                Assert.True(call.CallDate.Date == calls.First().CallDate.Date, "Получены некорректные данные (CallDate)");
                Assert.True(call.CallReport == calls.First().CallReport, "Получены некорректные данные (CallReport)");
                Assert.True(call.OrderCost == calls.First().OrderCost, "Получены некорректные данные (OrderCost)");

                call.CallId = calls.First().CallId;
            }

            call.CallDate = DateTime.Now.AddDays(-3);
            call.CallReport = "Был сделан заказ";
            call.OrderCost = 300;
            using (var resp = await client.PutAsJsonAsync($"api/persons/{person.PersonId}/calls", call))
            {
                Assert.True(resp.IsSuccessStatusCode, $"При обновлении отчета получен неожиданный ответ - {resp.StatusCode}");
            }
            using (var resp = await client.GetAsync($"api/persons/{person.PersonId}/calls"))
            {
                Assert.True(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
                var calls = await resp.Content.ReadAsAsync<IEnumerable<Call>>();
                Assert.True(calls != null, "После обновления записи не удалось загрузить данные");
                Assert.True(1 == calls.Count(), $"Неожиданное количество отчетов после добавления: {calls.Count()}");

                Assert.True(call.CallDate.Date == calls.First().CallDate.Date, "Получены некорректные данные (CallDate)");
                Assert.True(call.CallReport == calls.First().CallReport, "Получены некорректные данные (CallReport)");
                Assert.True(call.OrderCost == calls.First().OrderCost, "Получены некорректные данные (OrderCost)");
            }
        }
        [Fact]
        public async Task WebApi_DeleteCall()
        {
            using var client = GetClient();
            var person = await AddPersonAndGetItWithId();
            var call = new Call()
            {
                CallDate = DateTime.Now.AddDays(-7),
                CallReport = "Предложение по новой коллекции понравилось клиенту, но он решил заказать предыдущий товар"
            };
            using (var resp = await client.PostAsJsonAsync($"api/persons/{person.PersonId}/calls", call))
            {
                Assert.True(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
            }
            using (var resp = await client.GetAsync($"api/persons/{person.PersonId}/calls"))
            {
                Assert.True(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
                var calls = await resp.Content.ReadAsAsync<IEnumerable<Call>>();
                Assert.True(calls != null, "После обновления записи не удалось загрузить данные");
                Assert.True(1 == calls.Count(), $"Неожиданное количество отчетов после добавления: {calls.Count()}");

                call.CallId = calls.First().CallId;
            }
            using (var resp = await client.DeleteAsync($"api/persons/{person.PersonId}/calls/{call.CallId}"))
            {
                Assert.True(resp.IsSuccessStatusCode, $"При удалении отчета получен неожиданный ответ - {resp.StatusCode}");
            }
            using (var resp = await client.GetAsync($"api/persons/{person.PersonId}/calls"))
            {
                Assert.True(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
                var calls = await resp.Content.ReadAsAsync<IEnumerable<Call>>();
                Assert.True(calls != null, "После обновления записи не удалось загрузить данные");
                Assert.True(0 == calls.Count(), $"Неожиданное количество отчетов после удаления: {calls.Count()}");
            }
        }
        [Fact]
        public async Task WebApi_GetPageFilteredByMinDays()
        {
            using var client = GetClient();
            var person1 = await AddPersonAndGetItWithId();
            var call1 = new Call()
            {
                CallDate = DateTime.Now.AddDays(-30),
                CallReport = "Отчет для фильтрации"
            };
            var person2 = await AddPersonAndGetItWithId();
            person2.FirstName = person1.FirstName;
            using (var res = await client.PutAsJsonAsync("api/persons", person2)) { }
            var call2 = new Call()
            {
                CallDate = DateTime.Now.AddDays(-10),
                CallReport = "Отчет для фильтрации"
            };

            using (var res = await client.PostAsJsonAsync($"api/persons/{person1.PersonId}/calls", call1)) { }
            using (var res = await client.PostAsJsonAsync($"api/persons/{person2.PersonId}/calls", call2)) { }

            using (var res = await client.GetAsync($"api/persons?MinDaysAfterLastCall=30&PageSize=100&NameFilter={person1.FirstName}"))
            {
                Assert.True(res.IsSuccessStatusCode, $"Неожиданный код ответа {res.StatusCode}");
                var pList = await res.Content.ReadAsAsync<IEnumerable<Person>>();
                Assert.True(1 == pList.Count(), $"Страница не содержит ожидаемого количества данных {pList.Count()}");
                Assert.True(person1.FirstName == pList.First().FirstName, "Получены некорректные данные");
                Assert.True(person1.LastName == pList.First().LastName, "Получены некорректные данные");
            }
        }
    }
}
