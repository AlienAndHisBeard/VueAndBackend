namespace webapi.Models
{
    public class StopsRoot
    {
        public string? LastUpdate {  get; set; }
        public List<StopInfo>? Stops { get; set; }
    }

    public class StopInfo
    {
        public int StopId { get; set; }
        public string? StopCode { get; set; }
        public string? StopName { get; set; }
        public string? StopShortName { get; set; }
        public string? StopDesc { get; set; }
        public string? SubName { get; set; }
        public string? Date { get; set; }
        public int? ZoneId { get; set; }
        public string? ZoneName { get; set; }
        public bool? VirtualBool { get; set; }
        public bool? Nonpassenger { get; set; }
        public bool? Depot { get; set; }
        public bool? TicketZoneBorder { get; set; }
        public bool? OnDemand { get; set; }
        public string? ActivationDate { get; set; }
        public double? StopLat { get; set; }
        public double? StopLon { get; set; }
        public string? StopUrl { get; set; }
        public string? LocationType { get; set; }
        public string? ParentStation { get; set; }
        public string? StopTimezone { get; set; }
        public string? WheelchairBoarding { get; set; }
    }
}
