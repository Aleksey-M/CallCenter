using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CallCenter.Back.Data;

namespace CallCenter.Back.Pages.Persons
{
    public class DetailsModel : PageModel
    {
        private readonly CallCenter.Back.Data.DataBaseContext _context;

        public DetailsModel(CallCenter.Back.Data.DataBaseContext context)
        {
            _context = context;
        }

        public Person Person { get; set; }
        public List<Call> Calls { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Person = await _context.Persons.SingleOrDefaultAsync(m => m.PersonId == id);

            if (Person == null)
            {
                return NotFound();
            }
            Calls = await _context.Calls.Where(c => c.PersonId == Person.PersonId).OrderByDescending(c => c.CallDate).ToListAsync();
            return Page();
        }
    }
}
