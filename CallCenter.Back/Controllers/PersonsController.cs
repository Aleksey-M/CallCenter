using CallCenter.Back.Data;
using CallCenter.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CallCenter.Back.Controllers
{
    public class PersonsController : Controller
    {
        private readonly DataBaseContext _context;
        public PersonsController(DataBaseContext context)
        {
            _context = context;
        }

        [HttpGet, Route("api/persons/count")]
        public async Task<IActionResult> GetPersonsCount([FromQuery] PersonsFilterFields filters)
        {
            if (filters != null)
            {
                var validator = new PersonsFilterFieldsValidator();
                var valRes = validator.Validate(filters);
                if (!valRes.IsValid)
                {
                    return BadRequest();
                }
            }
            return Ok(await _context.GetPersonsCountAsync(filters ?? PersonsFilterFields.Default));
        }

        [HttpGet, Route("api/persons")]
        public async Task<IActionResult> GetPersons([FromQuery] PersonsFilterFields filters)
        {
            var validator = new PersonsFilterFieldsValidator();
            var valRes = validator.Validate(filters);
            if (valRes.IsValid)
            {
                var pList = await _context.GetPersonsListAsync(filters);
                return Ok(pList);
            }
            return BadRequest();
        }

        [HttpGet, Route("api/persons/{id}")]
        public async Task<IActionResult> GetPerson(Guid id)
        {
            var p = await _context.GetPerson(id);
            if (p == null) return NotFound();
            return Ok(p);
        }

        [HttpDelete, Route("api/persons/{id}")]
        public async Task<IActionResult> DeletePerson(Guid id)
        {
            var p = await _context.GetPerson(id);
            if (p == null) return NotFound();
            await _context.DeletePersonAsync(id);
            return Ok();
        }

        [HttpPost, Route("api/persons")]
        public async Task<IActionResult> AddPerson([FromBody] Person person)
        {
            var validator = new PersonValidator();
            var valRes = validator.Validate(person);
            if (valRes.IsValid)
            {
                _ = await _context.AddPersonAsync(person);
                return Ok();
            }
            return BadRequest();
        }

        [HttpPut, Route("api/persons")]
        public async Task<IActionResult> UpdatePerson([FromBody] Person person)
        {
            var validator = new PersonValidator();
            var valRes = validator.Validate(person);
            if (valRes.IsValid)
            {
                var p = await _context.GetPerson(person.PersonId);
                if (p == null) return NotFound();
                await _context.UpdatePersonAsync(person);
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost, Route("api/createtestdata")]
        public async Task<IActionResult> CreateTestData()
        {
            await DataHelper.AddTestData(_context);
            return Ok("Test data was added");
        }
    }
}