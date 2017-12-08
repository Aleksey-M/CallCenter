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
    public class IndexModel : PageModel
    {
        private readonly CallCenter.Back.Data.DataBaseContext _context;

        public IndexModel(CallCenter.Back.Data.DataBaseContext context)
        {
            _context = context;
        }

        public IList<Person> Persons { get;set; }

        public async Task OnGetAsync()
        {
            Persons = await _context.Persons.ToListAsync();
        }

        public async Task OnGetAddTestRecordsAsync()
        {
            DataHelper.AddTestData(_context);
            Persons = await _context.Persons.ToListAsync();
        }
    }
}
