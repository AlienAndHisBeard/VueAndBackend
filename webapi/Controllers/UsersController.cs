using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using webapi.Data;
using webapi.Models;

namespace webapi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ZtmDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client = new();

        public UsersController(ZtmDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

            // default user
            if (_context.Users.IsNullOrEmpty())
            {
                var user = new User { Login = "jkkp", Password = "1234", BusStops = "8227 8228 8229" };
                _context.Users.Add(user);
                _context.SaveChanges();
            }
        }

        // GET: api/Users
        [AllowAnonymous]
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
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (_context == null || _context.Users.IsNullOrEmpty())
            {
                return Problem("Entity set 'ZtmDbContext.Users' is null or empty.");
            }

            var result = await _context.Users.FirstAsync(e => e.Login == user.Login);
            if (result != null) { return NotFound(); }

            _context.Users.Add(new User() { Login = user.Login, Password = user.Password, BusStops = user.BusStops });
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostLogin", user);
        }

        // DELETE: api/Users
        [HttpDelete]
        public async Task<IActionResult> DeleteUser()
        {
            if (_context == null || _context.Users.IsNullOrEmpty())
            {
                return Problem("Entity set 'ZtmDbContext.Users' is null or empty.");
            }

            var login = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Users.FirstAsync(e => e.Login == login);

            if (user == null) { return NotFound(); }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Users/login
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        public async Task<ActionResult<string>> PostLogin(User login)
        {
            if (_context == null || _context.Users.IsNullOrEmpty())
            {
                return Problem("Entity set 'ZtmDbContext.Users' is null or empty.");
            }

            var user = await _context.Users.FirstAsync(e => e.Login == login.Login && e.Password == login.Password);
            if (user != null)
            {
                var securityKey = new
                    SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!));
                var credentials = new SigningCredentials(securityKey,
                    SecurityAlgorithms.HmacSha256);
                var claims = new List<Claim>
                {
                    new Claim("sub", login.Login!)
                };
                var token = new JwtSecurityToken(
                    _configuration["JwtSettings:Issuer"],
                    _configuration.GetSection("JwtSettings:Audience").Get<List<string>>()![1],
                    claims,
                    expires: DateTime.Now.AddMinutes(120),
                    signingCredentials: credentials);
                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            return NotFound();

        }

        // GET: api/Users/BusStopsAndDelays
        [Route("BusStopsAndDelays")]
        [HttpGet]
        public async Task<ActionResult<string>> GetUserBusStopsAndDelays()
        {
            if (_context == null || _context.Users.IsNullOrEmpty())
            {
                return Problem("Entity set 'ZtmDbContext.Users' is null or empty.");
            }
            var login = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Users.FirstAsync(e => e.Login == login);

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

        // GET: api/Users/BusStops
        [Route("BusStops")]
        [HttpGet]
        public async Task<ActionResult<string>> GetUserBusStops()
        {
            if (_context == null || _context.Users.IsNullOrEmpty())
            {
                return Problem("Entity set 'ZtmDbContext.Users' is null or empty.");
            }
            var login = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Users.FirstAsync(e => e.Login == login);

            if (user == null) { return NotFound(); }

            var stopsId = GetBusStops(user.BusStops);

            return System.Text.Json.JsonSerializer.Serialize(stopsId);
        }

        // POST: api/Users/AddUserBusStop
        [Route("AddUserBusStop")]
        [HttpPost]
        public async Task<IActionResult> AddUserBusStop([FromBody] int id)
        {
            if (_context == null || _context.Users.IsNullOrEmpty())
            {
                return Problem("Entity set 'ZtmDbContext.Users' is null or empty.");
            }
            var login = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Users.FirstAsync(e => e.Login == login);

            if (user == null) { return NotFound(); }

            var stopsId = GetBusStops(user.BusStops);

            if (!stopsId.Contains(id))
            {
                if (stopsId.Count > 0) { user.BusStops += " "; }
                user.BusStops += $"{id}";
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }

        // DELETE: api/Users/DeleteUserBusStop
        [Route("DeleteUserBusStop")]
        [HttpDelete]
        public async Task<IActionResult> DeleteUserBusStop([FromBody] int id)
        {
            if (_context == null || _context.Users.IsNullOrEmpty())
            {
                return Problem("Entity set 'ZtmDbContext.Users' is null or empty.");
            }
            var login = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Users.FirstAsync(e => e.Login == login);

            if (user == null) { return NotFound(); }

            var stopsId = GetBusStops(user.BusStops);

            if (stopsId.Contains(id))
            {
                stopsId = stopsId.Where(s => s != id).ToList();

                StringBuilder sb = new();
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

            if (busStopsStr!.Contains(' '))
            {
                return busStopsStr.Split(' ').Select(int.Parse).ToList();
            }

            busStops.Add(int.Parse(busStopsStr));
            return busStops;
        }

    }
}
