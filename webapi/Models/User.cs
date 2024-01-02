using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class User
    {
        [Key]
        public int Id { get; init; }
        public string? Login { get; set; }
        public string? Password { get; set; }
        public string? BusStops { get; set; }
    }
}
