using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using webapi.Data;
using webapi.Models;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ZtmDbContext _context;
        private readonly HttpClient _client = new();

        public UsersController(ZtmDbContext context)
        {
            _context = context;

            // default user
            if (_context.Users.IsNullOrEmpty())
            {
                var user = new User { Login = "jkkp", Password = "1234", BusStops = "8227 8228 8229" };
                _context.Users.Add(user);
                _context.SaveChanges();
            }
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetUsers()
        {
            if (_context.Users.IsNullOrEmpty())
            {
                return Problem("Entity set 'ZtmDbContext.Users' is null or empty.");
            }
            var users = await _context.Users.ToListAsync();
            return users.Select(u => u.Login!).ToList();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (_context == null || _context.Users.IsNullOrEmpty())
            {
              return Problem("Entity set 'ZtmDbContext.Users' is null or empty.");
            }

            _context.Users.Add(new User() { Login = user.Login, Password = user.Password, BusStops = user.BusStops });
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostLogin", user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (_context == null || _context.Users.IsNullOrEmpty())
            {
                return Problem("Entity set 'ZtmDbContext.Users' is null or empty.");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null) { return NotFound(); }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Users/login
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Route("login")]
        [HttpPost]
        public async Task<ActionResult<User>> PostLogin(User login)
        {
            if (_context == null || _context.Users.IsNullOrEmpty())
            {
                return Problem("Entity set 'ZtmDbContext.Users' is null or empty.");
            }

            var user = await _context.Users.FirstAsync(e => e.Login == login.Login && e.Password == login.Password);
            if (user != null)
            {
                return Ok(new { id = user.Id });
            }

            return NotFound();

        }

        // GET: api/Users/5/BusStopsAndDelays
        [Route("{id}/BusStopsAndDelays")]
        [HttpGet]
        public async Task<ActionResult<string>> GetUserBusStopsAndDelays(int id)
        {
            if (_context == null || _context.Users.IsNullOrEmpty())
            {
                return Problem("Entity set 'ZtmDbContext.Users' is null or empty.");
            }
            var user = await _context.Users.FindAsync(id);

            if (user == null) { return NotFound(); }

            var stopsId = GetBusStops(user.BusStops);

            //var stopsInfo = new List<(List<Delay>?, int)>();
            var stopsInfo = new Dictionary<int, List<Delay>>();

            foreach (var stopId in stopsId)
            {
                var response = await (await _client.GetAsync($"http://ckan2.multimediagdansk.pl/delays?stopId={stopId}")).Content.ReadAsStringAsync();
                var delays = JsonConvert.DeserializeObject<(string, List<Delay>)?>(response ?? "")?.Item2;

                delays ??= new List<Delay>();

                stopsInfo.TryAdd(stopId, delays);
                //stopsInfo.Add((delays, stopId));
            }
            return System.Text.Json.JsonSerializer.Serialize(stopsInfo);
        }

        // GET: api/Users/5/BusStops
        [Route("{id}/BusStops")]
        [HttpGet]
        public async Task<ActionResult<string>> GetUserBusStops(int id)
        {
            if (_context == null || _context.Users.IsNullOrEmpty())
            {
                return Problem("Entity set 'ZtmDbContext.Users' is null or empty.");
            }
            var user = await _context.Users.FindAsync(id);

            if (user == null) { return NotFound(); }

            var stopsId = GetBusStops(user.BusStops);

            return System.Text.Json.JsonSerializer.Serialize(stopsId);
        }

        // POST: api/Users/AddUserBusStop
        [Route("AddUserBusStop")]
        [HttpPost]
        public async Task<IActionResult> AddUserBusStop(int[] ids)
        {
            if (_context == null || _context.Users.IsNullOrEmpty())
            {
                return Problem("Entity set 'ZtmDbContext.Users' is null or empty.");
            }
            var user = await _context.Users.FindAsync(ids[0]);

            if (user == null) { return NotFound(); }

            var stopsId = GetBusStops(user.BusStops);

            if (!stopsId.Contains(ids[1]))
            {
                if(stopsId.Count > 0) { user.BusStops += " "; }
                user.BusStops += $"{ids[1]}";
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }

        // DELETE: api/Users/DeleteUserBusStop
        [Route("DeleteUserBusStop")]
        [HttpDelete]
        public async Task<IActionResult> DeleteUserBusStop(int[] ids)
        {
            if (_context == null || _context.Users.IsNullOrEmpty())
            {
                return Problem("Entity set 'ZtmDbContext.Users' is null or empty.");
            }
            var user = await _context.Users.FindAsync(ids[0]);

            if (user == null) { return NotFound(); }

            var stopsId = GetBusStops(user.BusStops);

            if (stopsId.Contains(ids[1]))
            {
                stopsId = stopsId.Where(s => s != ids[1]).ToList();

                StringBuilder sb = new StringBuilder();
                foreach (var stop in stopsId)
                {
                    sb.AppendFormat("{0} ", stop);
                }

                if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);

                user.BusStops = sb.ToString();

                await _context.SaveChangesAsync();
                return Ok();
            }
            return NotFound();
        }

        private static List<int> GetBusStops(string? busStopsStr)
        {
            var busStops = new List<int>();
            if (busStopsStr.IsNullOrEmpty()) { return busStops; }

            if(busStopsStr!.Contains(' '))
            {
                return busStopsStr.Split(' ').Select(int.Parse).ToList();
            }

            busStops.Add(int.Parse(busStopsStr));
            return busStops;
        }

    }
}
