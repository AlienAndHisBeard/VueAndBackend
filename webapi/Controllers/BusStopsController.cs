using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using webapi.Models;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusStopsController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly HttpClient _client = new();

        public BusStopsController(IMemoryCache cache)
        {
            _cache = cache;
        }

        // GET: api/BusStops
        [HttpGet]
        public async Task<ActionResult<string?>> GetStops()
        {
            var stopsJson = await _cache.GetOrCreateAsync($"stops", async stops =>
            {
                return await (await _client.GetAsync($"https://ckan.multimediagdansk.pl/dataset/c24aa637-3619-4dc2-a171-a23eec8f2172/resource/4c4025f0-01bf-41f7-a39f-d156d201b82b/download/stops.json")).Content.ReadAsStringAsync();
            });

            if ( stopsJson == null) { return NotFound(); }

            var response = JsonConvert.DeserializeObject<Dictionary<string, StopsRoot>>(stopsJson ?? "");

            var stopsInfo = response?.First().Value.Stops;

            if ( stopsInfo == null ) {  return NotFound(); }

            foreach (var stop in stopsInfo)
            {
                _cache!.Set($"stopInfo{stop.StopId}", stop);
            }
            return System.Text.Json.JsonSerializer.Serialize(stopsInfo);
        }

        // GET: api/BusStops/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<StopInfo>> GetStops(int id)
        {
            if (_cache!.Get($"stopInfo{id}") is StopInfo stop) { return stop; }

            await GetStops();

            if (_cache!.Get($"stopInfo{id}") is StopInfo stopReloaded) { return stopReloaded; }

            return NotFound();
        }

        [Route("delays/{id}")]
        [HttpGet]
        public async Task<ActionResult<string?>> GetDelays(int id)
        {
            return await (await _client.GetAsync($"http://ckan2.multimediagdansk.pl/delays?stopId={id}")).Content.ReadAsStringAsync();
        }
    }
}
