using CallCenter.Data;
using CallCenter.Data.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CallCenter.WebApp.Controllers
{
    public class CallsController : Controller
    {
        private readonly DataBaseContext _context;
        public CallsController(DataBaseContext context)
        {
            _context = context;            
        }

        [HttpDelete, Route("api/persons/{pid}/calls/{cid}")]
        public async Task<IActionResult> DeleteCall(Guid pid, Guid cid)
        {
            var pers = _context.GetPerson(pid);
            if (pers == null) return BadRequest();
            var calls = _context.GetCalls(pid);
            if (calls.FirstOrDefault(c => c.CallId == cid) == null) return BadRequest();
            await _context.DeleteCallAsync(cid);
            return Ok();
        }

        [HttpPost, Route("api/persons/{pid}/calls")]
        public async Task<IActionResult> AddCall(Guid pid, [FromBody] Call call)
        {
            var validator = new CallValidator();
            var valRes = validator.Validate(call);
            if (valRes.IsValid)
            {
                await _context.AddCallAsync(call, pid);
                return Ok();
            }
            return BadRequest();
        }

        [HttpPut, Route("api/persons/{pid}/calls")]
        public async Task<IActionResult> UpdateCall(Guid pid, [FromBody] Call call)
        {
            var pers = _context.GetPerson(pid);
            if (pers == null) return BadRequest();

            if (!_context.GetCalls(pid).Exists(c => c.CallId == call.CallId)) return BadRequest();

            var validator = new CallValidator();
            var valRes = validator.Validate(call);
            if (valRes.IsValid)
            {
                await _context.UpdateCallAsync(call);
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet, Route("api/persons/{pid}/calls")]
        public async Task<IActionResult> GetCalls(Guid pid)
        {
            if (_context.GetPerson(pid) == null) return BadRequest();
            return Ok(await Task.Run(() => _context.GetCalls(pid)));
        }
    }
}