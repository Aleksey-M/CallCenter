using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using CallCenter.Back.Data;
using Xunit;

namespace CallCenter.UI.Tests
{    
    public class WebApiTests
    {
#warning Перед запуском тестов должно быть запущено Web-приложение и обновлен номер порта. Используется рабочая база данных        
        private string BaseAddress { get { return "http://localhost:63964/"; } }
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
        private void DeletePersonFromBaseIfExists(string personName)
        {
            using (var client = GetClient())
            {
                using (var response = client.GetAsync($"api/persons?NameFilter={personName}").Result)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var pList = response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                        foreach(var pers in pList)
                        {
                            var r = client.DeleteAsync($"api/persons/{pers.Id}").Result;
                        }
                    }
                }
            }
        }
        
        private Person AddPersonAndGetItWithId()
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
            DeletePersonFromBaseIfExists(person.FirstName);

            using (var client = GetClient())
            {
                using(var response = client.PostAsJsonAsync("api/persons", person).Result)
                {
                    if (!response.IsSuccessStatusCode) return null;
                }
                using(var response = client.GetAsync($"api/persons?NameFilter={person.FirstName}").Result)
                {
                    if (!response.IsSuccessStatusCode) return null;
                    var list = response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                    if (list == null) return null;
                    person.PersonId = list.First().Id;
                }
            }            
            return person;
        }

        [Fact]
        public void WebApi_GetCount()
        {
            using (var client = GetClient())
            {
                using (var response = client.GetAsync("api/persons/count").Result)
                {
                    Assert.True(response.IsSuccessStatusCode, $"Неправильный статусный код при запросе количества записей: {response.StatusCode}");
                    int recCount = response.Content.ReadAsAsync<int>().Result;
                    Assert.True(recCount >= 0, "Отрицательное значение Count");
                }
            }            
        }
        
        [Fact]
        public void WebApi_AddPerson()
        {
            using (var client = GetClient())
            {
                int pCount, newCount; 
                using (var response = client.GetAsync("api/persons/count").Result)
                {
                    Assert.True(response.IsSuccessStatusCode, $"Неправильный статусный код при запросе количества записей: {response.StatusCode}");
                    pCount = response.Content.ReadAsAsync<int>().Result;                    
                }

                var person = new Person()
                {
                    PersonId = Guid.Empty,
                    BirthDate = DateTime.Parse("20.05.1990"),
                    FirstName = "Евгения",
                    Patronymic = "Федоровна",
                    LastName = "Полкина",
                    Gender = Gender.Female,
                    PhoneNumber = "+380997586636"                    
                };
                using (var response = client.PostAsJsonAsync("api/persons", person).Result)
                {
                    Assert.True(response.IsSuccessStatusCode, $"Неправильный статусный код: {response.StatusCode}");                    
                }

                using (var response = client.GetAsync("api/persons/count").Result)
                {
                    Assert.True(response.IsSuccessStatusCode, $"Неправильный статусный код при запросе количества записей: {response.StatusCode}");
                    newCount = response.Content.ReadAsAsync<int>().Result;
                }
                Assert.True(pCount + 1 == newCount, $"Запись не добавлена: До добавления записей - {pCount},  после добавления - {newCount}");
            }                     
        }
        
        [Fact]
        public void WebApi_GetLastPageWithOnePerson()
        {
            using(var client = GetClient())
            {
                int pCount;
                using (var response = client.GetAsync("api/persons/count").Result)
                {
                    Assert.True(response.IsSuccessStatusCode, $"Неправильный статусный код при запросе количества записей: {response.StatusCode}");
                    pCount = response.Content.ReadAsAsync<int>().Result;
                }

                Assert.True(pCount != 0, "Для выполнения теста недостаточно записей в базе");

                using(var response = client.GetAsync($"api/persons?PageSize=1&PageNo=/{pCount}").Result)
                {
                    Assert.True(response.IsSuccessStatusCode, $"Неправильный код ответа при запросе страницы - {response.StatusCode}");
                    var page = response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                    Assert.True(page != null, "Пустй ответ при запросе страницы");
                    Assert.True(page.Count() == 1, $"Страница не содержит ожидаемого количества данных: {page.Count()}");
                }
            }
        }
        [Fact]
        public void WebApi_GetFirstPageWithThreePersons()
        {
            using (var client = GetClient())
            {
                int pCount;
                using (var response = client.GetAsync("api/persons/count").Result)
                {
                    Assert.True(response.IsSuccessStatusCode, $"Неправильный статусный код при запросе количества записей: {response.StatusCode}");
                    pCount = response.Content.ReadAsAsync<int>().Result;
                }

                Assert.True(pCount >=3, "Для выполнения теста недостаточно записей в базе");

                using (var response = client.GetAsync("api/persons?PageSize=3&PageNo=1").Result)
                {
                    Assert.True(response.IsSuccessStatusCode, $"Неправильный код ответа при запросе страницы - {response.StatusCode}");
                    var page = response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                    Assert.True(page != null, "Пустй ответ при запросе страницы");
                    Assert.True(page.Count() == 3, $"Страница не содержит ожидаемого количества данных: {page.Count()}");
                }
            }
        }
        [Fact]
        public void WebApi_GetPersonById()
        {
            using(var client = GetClient())
            {
                var person = new Person() { FirstName = $"Name_{Guid.NewGuid().ToString().Substring(0, 8)}", PhoneNumber="test number" };                
                using (var response = client.PostAsJsonAsync("api/persons", person).Result)
                {
                    Assert.True(response.IsSuccessStatusCode, $"Неправильный статусный код при добавлении записи: {response.StatusCode}");
                }

                using (var response = client.GetAsync($"api/persons?NameFilter={Uri.EscapeDataString(person.FirstName)}").Result)
                {
                    Assert.True(response.IsSuccessStatusCode, $"Неправильный код ответа при запросе страницы - {response.StatusCode}");
                    var page = response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                    Assert.True(page != null, "Пустой ответ при запросе страницы");
                    Assert.True(page.Count() == 1, $"Страница не содержит ожидаемого количества данных: {page.Count()}");
                    person.PersonId = page.First().Id;
                }

                using (var response = client.GetAsync($"api/persons/{Uri.EscapeDataString(person.PersonId.ToString())}").Result)
                {
                    Assert.True(response.IsSuccessStatusCode, $"Неправильный код ответа при запросе страницы - {response.StatusCode}");
                    var pers = response.Content.ReadAsAsync<Person>().Result;
                    Assert.True(pers != null, $"Запись не найдена по id - {person.PersonId}");
                }
            }
        }
        [Fact]
        public void WebApi_GetPageFilteredByName()
        {
            using (var client = GetClient())
            {                
                var person = new Person() {
                    FirstName = $"Name_{Guid.NewGuid().ToString().Substring(0, 8)}",
                    LastName = $"Name_{Guid.NewGuid().ToString().Substring(0, 8)}",
                    Patronymic = $"Name_{Guid.NewGuid().ToString().Substring(0, 8)}",
                    PhoneNumber = "phone number" };
                using (var response = client.PostAsJsonAsync("api/persons", person).Result)
                {
                    Assert.True(response.IsSuccessStatusCode, $"Неправильный статусный код при добавлении записи: {response.StatusCode}");
                }
                
                using (var response = client.GetAsync($"api/persons?NameFilter={person.FirstName}").Result)
                {
                    Assert.True(response.IsSuccessStatusCode, $"Неправильный код ответа при запросе страницы - {response.StatusCode}");
                    var page = response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                    Assert.True(page != null, "Пустй ответ при запросе страницы");
                    Assert.True(page.Count() == 1, $"Страница не содержит ожидаемого количества данных: {page.Count()}");
                    Assert.True(person.FirstName == page.First().FirstName, "Получены некорректные данные (FirstName)");
                }

                using (var response = client.GetAsync($"api/persons?NameFilter={person.LastName}").Result)
                {
                    Assert.True(response.IsSuccessStatusCode, $"Неправильный код ответа при запросе страницы - {response.StatusCode}");
                    var page = response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                    Assert.True(page != null, "Пустй ответ при запросе страницы");
                    Assert.True(page.Count() == 1, $"Страница не содержит ожидаемого количества данных: {page.Count()}");
                    Assert.True(person.LastName == page.First().LastName, "Получены некорректные данные (LastName)");
                }

                using (var response = client.GetAsync($"api/persons?NameFilter={person.Patronymic}").Result)
                {
                    Assert.True(response.IsSuccessStatusCode, $"Неправильный код ответа при запросе страницы - {response.StatusCode}");
                    var page = response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                    Assert.True(page != null, "Пустй ответ при запросе страницы");
                    Assert.True(page.Count() == 1, $"Страница не содержит ожидаемого количества данных: {page.Count()}");
                    Assert.True(person.Patronymic == page.First().Patronymic, "Получены некорректные данные (Patronymic)");
                }
            }
        }
        [Fact]
        public void WebApi_GetPageFilteredByGender()
        {
            using(var client = GetClient())
            {
                int allCount, maleCount, femaleCount, pCount;
                using (var response = client.GetAsync($"api/persons?Gender={Gender.All}&pagesize=500").Result)
                {
                    Assert.True(response.IsSuccessStatusCode, $"Код запроса отфильрованного списка (Gender.All) - {response.StatusCode}");
                    var page = response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                    allCount = page.Count();
                    Assert.True(allCount > 0, "Недостаточно записей в базе для тестирования");
                }
                using (var response = client.GetAsync($"api/persons?Gender={Gender.Female}&pagesize=500").Result)
                {
                    Assert.True(response.IsSuccessStatusCode, $"Код запроса отфильрованного списка (Gender.Female) - {response.StatusCode}");
                    var page = response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                    femaleCount = page.Count();                    
                }
                using (var response = client.GetAsync($"api/persons?Gender={Gender.Male}&pagesize=500").Result)
                {
                    Assert.True(response.IsSuccessStatusCode, $"Код запроса отфильрованного списка (Gender.Male) - {response.StatusCode}");
                    var page = response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                    maleCount = page.Count();                    
                }
                using (var response = client.GetAsync("api/persons/count").Result)
                {
                    Assert.True(response.IsSuccessStatusCode, $"Неправильный статусный код при запросе количества записей: {response.StatusCode}");
                    pCount = response.Content.ReadAsAsync<int>().Result;
                }
                Assert.True(allCount == pCount, $"Ошибка в определении пола. All:{allCount}; Records Count:{pCount};");
            }
        }
        [Fact]
        public void WebApi_GetPageFilteredByMaxAge()
        {
            DeletePersonFromBaseIfExists("MaxAgeTestUser");
            using (var client = GetClient())
            {
                var person = new Person() { FirstName = "MaxAgeTestUser", PhoneNumber = "444-444", BirthDate = DateTime.Now.AddYears(-1) };
                DeletePersonFromBaseIfExists(person.FirstName);
                Thread.Sleep(1000);
                var res = client.PostAsJsonAsync("api/persons", person).Result;
                
                using(res = client.GetAsync("api/persons?MaxAge=1").Result)
                {
                    Assert.True(res.IsSuccessStatusCode, $"Код ответа не положительный- {res.StatusCode}");
                    var pList = res.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                    Assert.True(pList.Count() == 1, $"Страница не содержит ожидаемого количества данных: {pList.Count()}");
                    Assert.True(person.FirstName == pList.First().FirstName, "Получены некорректные данные");
                }
            }
        }
        [Fact]
        public void WebApi_GetPageFilteredByMinAge()
        {
            using (var client = GetClient())
            {
                var person = new Person() { FirstName = "MinAgeTestUser", PhoneNumber = "444-444", BirthDate = DateTime.Now.AddYears(-100) };
                DeletePersonFromBaseIfExists(person.FirstName);

                using (var res = client.PostAsJsonAsync("api/persons", person).Result) { }

                using (var res = client.GetAsync("api/persons?MinAge=100&MaxAge=101").Result)
                {
                    Assert.True(res.IsSuccessStatusCode, $"Код ответа не положительный - {res.StatusCode}");
                    var pList = res.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                    Assert.True(pList.Count() == 1, "Страница не содержит ожидаемого количества данных");
                    Assert.True(person.FirstName == pList.First().FirstName, "Получены некорректные данные");
                }
            }
        }       
        [Fact]
        public void WebApi_UpdatePerson()
        {
            using (var client = GetClient())
            {
                var person = new Person()
                {
                    FirstName = "UpdateName",
                    LastName = "UpdateName2",
                    Patronymic = "UpdatePatronymic",
                    BirthDate = DateTime.Now.AddYears(-30),
                    Gender = Gender.All,
                    PhoneNumber = "1111111111"
                };

                DeletePersonFromBaseIfExists(person.FirstName);
                using (var res = client.PostAsJsonAsync("api/persons", person).Result) { }

                using (var res = client.GetAsync($"api/persons?NameFilter={person.FirstName}").Result)
                {
                    Assert.True(res.IsSuccessStatusCode, $"Код ответа не положительный - {res.StatusCode}");
                    var pList = res.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
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
                DeletePersonFromBaseIfExists(person.FirstName);
                using (var res = client.PutAsJsonAsync($"api/persons", person).Result) { }

                Person personById, personByName;
                personByName = new Person();
                using (var res = client.GetAsync($"api/persons/{person.PersonId}").Result)
                {
                    personById = res.Content.ReadAsAsync<Person>().Result;                    
                }
                using (var res = client.GetAsync($"api/persons?NameFilter={person.FirstName}").Result)
                {
                    var list = res.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                    Assert.True(1 == list.Count(), "Страница не содержит ожидаемого количества данных");
                    personByName.PersonId = list.First().Id;                    
                }

                using (var res = client.GetAsync($"api/persons/{personByName.PersonId}").Result)
                {
                    personByName = res.Content.ReadAsAsync<Person>().Result;                    
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
        }

        [Fact]
        public void WebApi_DeletePerson()
        {
            using (var client = GetClient())
            {
                var person = new Person() { FirstName="DeletingTest", PhoneNumber="77777777"};

                using (var res = client.PostAsJsonAsync("api/persons", person).Result) { }

                using (var res = client.GetAsync($"api/persons?NameFilter={person.FirstName}").Result)
                {
                    Assert.True(res.IsSuccessStatusCode, $"Код ответа не положительный - {res.StatusCode}");
                    var resList = res.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                    Assert.True(1 == resList.Count(), "Страница не содержит ожидаемого количества данных");
                    person.PersonId = resList.First().Id;
                }

                using (var res = client.DeleteAsync($"api/persons/{person.PersonId}").Result)
                {
                    Assert.True(res.IsSuccessStatusCode, $"Код ответа не положительный - {res.StatusCode}");
                }
                using (var res = client.GetAsync($"api/persons/{person.PersonId}").Result)
                {
                    Assert.True(res.StatusCode == HttpStatusCode.NotFound, "Запись не удалена");
                }
            }
        }
              
        [Fact]
        public void WebApi_AddCall()
        {
            using (var client = GetClient())
            {
                var person = AddPersonAndGetItWithId();
                var call = new Call() {
                    CallDate = DateTime.Now.AddDays(-10),
                    CallReport = "Звонок прошел на удивление хорошо",
                    OrderCost = 489.54                    
                };
                using (var resp = client.PostAsJsonAsync($"api/persons/{person.PersonId}/calls", call).Result)
                {
                    Assert.True(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
                }
                Guid fcId;
                using (var resp = client.GetAsync($"api/persons/{person.PersonId}/calls").Result)
                {
                    Assert.True(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
                    var c = resp.Content.ReadAsAsync<IEnumerable<Call>>().Result;
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
                using (var resp = client.PostAsJsonAsync($"api/persons/{person.PersonId}/calls", call).Result)
                {
                    Assert.True(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
                }
                using (var resp = client.GetAsync($"api/persons/{person.PersonId}/calls").Result)
                {
                    Assert.True(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
                    var c = resp.Content.ReadAsAsync<IEnumerable<Call>>().Result;
                    Assert.True(c != null, "После обновления записи не удалось загрузить данные");
                    Assert.True(2 == c.Count(), $"Неожиданное количество отчетов после добавления: {c.Count()}");

                    Assert.True(call.CallDate.Date == c.FirstOrDefault(cl=>cl.CallId != fcId).CallDate.Date, "Получены некорректные данные (CallDate)");
                    Assert.True(call.CallReport == c.FirstOrDefault(cl => cl.CallId != fcId).CallReport, "Получены некорректные данные (CallReport)");
                    Assert.True(call.OrderCost == c.FirstOrDefault(cl => cl.CallId != fcId).OrderCost, "Получены некорректные данные (OrderCost)");                    
                }
            }
        }
        [Fact]
        public void WebApi_UpdateCall()
        {
            using (var client = GetClient())
            {
                var person = AddPersonAndGetItWithId();
                var call = new Call()
                {
                    CallDate = DateTime.Now.AddDays(-7),
                    CallReport = "Предложение по новой коллекции понравилось клиенту, но он решил заказать предыдущий товар",
                    OrderCost = 229.99                                       
                };
                using (var resp = client.PostAsJsonAsync($"api/persons/{person.PersonId}/calls", call).Result)
                {
                    Assert.True(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
                }
                using (var resp = client.GetAsync($"api/persons/{person.PersonId}/calls").Result)
                {
                    Assert.True(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
                    var calls = resp.Content.ReadAsAsync<IEnumerable<Call>>().Result;
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
                using (var resp = client.PutAsJsonAsync($"api/persons/{person.PersonId}/calls", call).Result)
                {
                    Assert.True(resp.IsSuccessStatusCode, $"При обновлении отчета получен неожиданный ответ - {resp.StatusCode}");
                }
                using (var resp = client.GetAsync($"api/persons/{person.PersonId}/calls").Result)
                {
                    Assert.True(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
                    var calls = resp.Content.ReadAsAsync<IEnumerable<Call>>().Result;
                    Assert.True(calls != null, "После обновления записи не удалось загрузить данные");
                    Assert.True(1 == calls.Count(), $"Неожиданное количество отчетов после добавления: {calls.Count()}");

                    Assert.True(call.CallDate.Date == calls.First().CallDate.Date, "Получены некорректные данные (CallDate)");
                    Assert.True(call.CallReport == calls.First().CallReport, "Получены некорректные данные (CallReport)");
                    Assert.True(call.OrderCost == calls.First().OrderCost, "Получены некорректные данные (OrderCost)");                                   
                }
            }
        }
        [Fact]
        public void WebApi_DeleteCall()
        {
            using (var client = GetClient())
            {
                var person = AddPersonAndGetItWithId();
                var call = new Call()
                {
                    CallDate = DateTime.Now.AddDays(-7),
                    CallReport = "Предложение по новой коллекции понравилось клиенту, но он решил заказать предыдущий товар"                   
                };
                using (var resp = client.PostAsJsonAsync($"api/persons/{person.PersonId}/calls", call).Result)
                {
                    Assert.True(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
                }
                using (var resp = client.GetAsync($"api/persons/{person.PersonId}/calls").Result)
                {
                    Assert.True(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
                    var calls = resp.Content.ReadAsAsync<IEnumerable<Call>>().Result;
                    Assert.True(calls != null, "После обновления записи не удалось загрузить данные");
                    Assert.True(1 == calls.Count(), $"Неожиданное количество отчетов после добавления: {calls.Count()}");
                                        
                    call.CallId = calls.First().CallId;
                }
                using (var resp = client.DeleteAsync($"api/persons/{person.PersonId}/calls/{call.CallId}").Result)
                {
                    Assert.True(resp.IsSuccessStatusCode, $"При удалении отчета получен неожиданный ответ - {resp.StatusCode}");
                }
                using (var resp = client.GetAsync($"api/persons/{person.PersonId}/calls").Result)
                {
                    Assert.True(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
                    var calls = resp.Content.ReadAsAsync<IEnumerable<Call>>().Result;
                    Assert.True(calls != null, "После обновления записи не удалось загрузить данные");
                    Assert.True(0 == calls.Count(), $"Неожиданное количество отчетов после удаления: {calls.Count()}");
                }
            }
        }
        [Fact]
        public void WebApi_GetPageFilteredByMinDays()
        {
            using (var client = GetClient())
            {
                var person1 = AddPersonAndGetItWithId();
                var call1 = new Call()
                {
                    CallDate = DateTime.Now.AddDays(-30),
                    CallReport = "Отчет для фильтрации"                   
                };
                var person2 = AddPersonAndGetItWithId();
                person2.FirstName = person1.FirstName;
                using (var res = client.PutAsJsonAsync("api/persons", person2).Result) { }
                var call2 = new Call()
                {
                    CallDate = DateTime.Now.AddDays(-10),
                    CallReport = "Отчет для фильтрации"                    
                };

                using (var res = client.PostAsJsonAsync($"api/persons/{person1.PersonId}/calls", call1).Result) { }
                using (var res = client.PostAsJsonAsync($"api/persons/{person2.PersonId}/calls", call2).Result) { }
                
                using (var res = client.GetAsync($"api/persons?MinDaysAfterLastCall=30&PageSize=100&NameFilter={person1.FirstName}").Result)
                {
                    Assert.True(res.IsSuccessStatusCode, $"Неожиданный код ответа {res.StatusCode}");
                    var pList = res.Content.ReadAsAsync<IEnumerable<Person>>().Result;
                    Assert.True(1 == pList.Count(), $"Страница не содержит ожидаемого количества данных {pList.Count()}");
                    Assert.True(person1.FirstName == pList.First().FirstName, "Получены некорректные данные");
                    Assert.True(person1.LastName == pList.First().LastName, "Получены некорректные данные");
                }
            }            
        }        
    }
}
