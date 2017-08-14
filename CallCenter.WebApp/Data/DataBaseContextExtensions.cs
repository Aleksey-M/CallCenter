using CallCenter.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CallCenter.Data
{

    public class PersonsFilterFields
    {
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        public Gender Gender { get; set; } = Gender.All;
        public string NameFilter { get; set; } = string.Empty;
        public int? MaxAge { get; set; } = null;
        public int? MinAge { get; set; } = null;
        public int MinDaysAfterLastCall { get; set; } = 0;
        public override string ToString()
        {
            return $"Filter fields: PageNo:{PageNo}; PageSize:{PageSize}; Gender:{Gender}; NameFilter:{NameFilter}; MaxAge:{MaxAge}; MinAge:{MinAge}; MinDaysAfterLastCall:{MinDaysAfterLastCall}";
        }

        public static PersonsFilterFields Default => new PersonsFilterFields();
    }

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

        public static Person GetPerson(this DataBaseContext context, Guid personId)
        {
            var pers = context.Persons.Find(personId);
            return pers;
        }

        public static async Task<Guid> AddCallAsync(this DataBaseContext context, Call call, Guid personId)
        {
            var pers = context.Persons.Find(personId);
            if (pers != null)
            {                
                call.PersonId = pers.PersonId;
                call.Person = pers;
                context.Calls.Add(call);
                await context.SaveChangesAsync();
                return call.CallId;
            }
            else return Guid.Empty;
        }
        
        public static async Task DeletePersonAsync(this DataBaseContext context, Guid personId)
        {
            var pers = context.Persons.Find(personId);
            if (pers != null)
            {
                context.Persons.Remove(pers);
                await context.SaveChangesAsync();
            }
        }

        public static async Task DeleteCallAsync(this DataBaseContext context, Guid callId)
        {
            var dbCall = context.Calls.Find(callId);
            if (dbCall != null)
            {
                context.Calls.Remove(dbCall);
                await context.SaveChangesAsync();
            }
        }

        public static Task UpdateCallAsync(this DataBaseContext context, Call call)
        {
            var dbCall = context.Calls.Find(call.CallId);
            if (dbCall == null)
            {
                throw new Exception($"Отчет с Id = {call.CallId} не найден в базе данных");
            }

            dbCall.OrderCost = call.OrderCost;
            dbCall.CallDate = call.CallDate;
            dbCall.CallReport = call.CallReport;

            return context.SaveChangesAsync();
        }

        public static Task<int> PersonsCountAsync(this DataBaseContext context)
        {
            return context.Persons.CountAsync();
        }

        public static List<Call> GetCalls(this DataBaseContext context, Guid personId)
        {
            var calls = context.Calls.Where(c => c.PersonId == personId);
            return calls?.ToList() ?? new List<Call>();
        }
#region Filtering
        private static IQueryable<Person> GetPersonQuery(this DataBaseContext context, PersonsFilterFields filterFields)
        {
            IQueryable<Person> persons;
            if (filterFields.MinDaysAfterLastCall > 0) {
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
                persons = persons.Where(p => 
                    (!string.IsNullOrWhiteSpace(p.FirstName) &&
                    p.FirstName.IndexOf(filterFields.NameFilter, StringComparison.OrdinalIgnoreCase) != -1)
                    ||
                    (!string.IsNullOrWhiteSpace(p.LastName) &&
                    p.LastName.IndexOf(filterFields.NameFilter, StringComparison.OrdinalIgnoreCase) != -1)
                    ||
                    (!string.IsNullOrWhiteSpace(p.Patronymic) &&
                    p.Patronymic.IndexOf(filterFields.NameFilter, StringComparison.OrdinalIgnoreCase) != -1));
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
