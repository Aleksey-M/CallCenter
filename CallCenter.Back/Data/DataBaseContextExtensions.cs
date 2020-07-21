using CallCenter.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CallCenter.Back.Data
{
    public static class DataBaseContextExtensions
    {
        public static async Task<Guid> AddPersonAsync(this DataBaseContext context, Person person)
        {
            context.Persons.Add(person);
            await context.SaveChangesAsync();
            return person.PersonId;
        }

        public static async Task UpdatePersonAsync(this DataBaseContext context, Person person)
        {
            var pers = context.Persons.Find(person.PersonId);
            if (pers != null)
            {
                pers.BirthDate = person.BirthDate;
                pers.FirstName = person.FirstName;
                pers.LastName = person.LastName;
                pers.Patronymic = person.Patronymic;
                pers.Gender = person.Gender;
                pers.PhoneNumber = person.PhoneNumber;

                await context.SaveChangesAsync();
            }
        }

        public static async Task<Person> GetPerson(this DataBaseContext context, Guid personId)
        {
            var pers = await context.Persons.FindAsync(personId);
            return pers;
        }

        public static async Task<Guid> AddCallAsync(this DataBaseContext context, Call call, Guid personId)
        {
            var pers = await context.Persons.FindAsync(personId);
            if (pers != null)
            {
                call.PersonId = pers.PersonId;
                context.Calls.Add(call);
                await context.SaveChangesAsync();
                return call.CallId;
            }
            else return Guid.Empty;
        }

        public static async Task DeletePersonAsync(this DataBaseContext context, Guid personId)
        {
            var pers = await context.Persons.FindAsync(personId);
            if (pers != null)
            {
                context.Persons.Remove(pers);
                await context.SaveChangesAsync();
            }
        }

        public static async Task DeleteCallAsync(this DataBaseContext context, Guid callId)
        {
            var dbCall = await context.Calls.FindAsync(callId);
            if (dbCall != null)
            {
                context.Calls.Remove(dbCall);
                await context.SaveChangesAsync();
            }
        }

        public static async Task<int> UpdateCallAsync(this DataBaseContext context, Call call)
        {
            var dbCall = await context.Calls.FindAsync(call.CallId);
            if (dbCall == null)
            {
                throw new Exception($"Отчет с Id = {call.CallId} не найден в базе данных");
            }

            dbCall.OrderCost = call.OrderCost;
            dbCall.CallDate = call.CallDate;
            dbCall.CallReport = call.CallReport;

            return await context.SaveChangesAsync();
        }

        public static Task<int> PersonsCountAsync(this DataBaseContext context)
        {
            return context.Persons.CountAsync();
        }

        public static async Task<List<Call>> GetCalls(this DataBaseContext context, Guid personId)
        {
            var calls = await context.Calls.Where(c => c.PersonId == personId).ToListAsync();
            return calls;
        }
        #region Filtering
        private static IQueryable<Person> GetPersonQuery(this DataBaseContext context, PersonsFilterFields filterFields)
        {
            IQueryable<Person> persons;
            if (filterFields.MinDaysAfterLastCall > 0)
            {
                var lastCallDate = DateTime.Now.AddDays(-filterFields.MinDaysAfterLastCall);
                persons = context.Persons.Where(p => p.Calls.Count == 0 || p.Calls.Max(c => c.CallDate) <= lastCallDate).OrderBy(p => p.LastName);
            }
            else
            {
                persons = context.Persons.OrderBy(p => p.LastName);
            }
            if (filterFields.Gender != Gender.All)
            {
                persons = persons.Where(p => p.Gender == filterFields.Gender);
            }
            if (filterFields.MinAge != null)
            {
                persons = persons.Where(p => p.BirthDate != null).Where(p => p.BirthDate != null && DateTime.Now.Year - p.BirthDate.Value.Year >= filterFields.MinAge.Value);
            }
            if (filterFields.MaxAge != null)
            {
                persons = persons.Where(p => p.BirthDate != null).Where(p => p.BirthDate != null && DateTime.Now.Year - p.BirthDate.Value.Year <= filterFields.MaxAge.Value);
            }

            if (!string.IsNullOrWhiteSpace(filterFields.NameFilter))
            {
                var names = context.Persons
                    .ToList()
                    .Where(p =>
                    (!string.IsNullOrWhiteSpace(p.FirstName) &&
                    p.FirstName.IndexOf(filterFields.NameFilter, StringComparison.OrdinalIgnoreCase) != -1)
                    ||
                    (!string.IsNullOrWhiteSpace(p.LastName) &&
                    p.LastName.IndexOf(filterFields.NameFilter, StringComparison.OrdinalIgnoreCase) != -1)
                    ||
                    (!string.IsNullOrWhiteSpace(p.Patronymic) &&
                    p.Patronymic.IndexOf(filterFields.NameFilter, StringComparison.OrdinalIgnoreCase) != -1))
                    .Select(p => p.PersonId)
                    .ToList();

                persons = persons.Where(p => names.Contains(p.PersonId));
            }

            return persons;
        }

        public static Task<List<Person>> GetPersonsAsync(this DataBaseContext context, PersonsFilterFields filterFields)
        {
            return context.GetPersonQuery(filterFields).Skip((filterFields.PageNo - 1) * filterFields.PageSize).Take(filterFields.PageSize).ToListAsync();
        }

        public static Task<int> GetPersonsCountAsync(this DataBaseContext context, PersonsFilterFields filterFields)
        {
            return context.GetPersonQuery(filterFields).CountAsync();
        }
        #endregion
        public static async Task<List<PersonsListItem>> GetPersonsListAsync(this DataBaseContext context, PersonsFilterFields filterFields)
        {
            var pList = await context.GetPersonsAsync(filterFields);
            return pList.AsParallel().Select(p => new PersonsListItem { Id = p.PersonId, FirstName = p.FirstName, LastName = p.LastName, Patronymic = p.Patronymic }).ToList();
        }

    }
}
