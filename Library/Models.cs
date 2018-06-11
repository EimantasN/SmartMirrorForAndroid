using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public class TemperatureNow
    {
        public string image { get; set; }
        public string Wind { get; set; }
        public string Pressure { get; set; }
        public string Humidity { get; set; }
        public string Water { get; set; }
        public string Sunrise { get; set; }
        public string Sunset { get; set; }
        public string realFeel { get; set; }
        public string Temperature { get; set; }
        public string dataRecieved { get; set; }
    }

    public class LinkomanijosData
    {
        public string name { get; set; }
        public string subtext { get; set; }
        public string date { get; set; }
        public string size { get; set; }
        public string downloaded { get; set; }
        public string seeder { get; set; }
    }

    public class TrafiListModel
    {
        public string EndStreet { get; set; }
        public string EndTime { get; set; }
        public string Image { get; set; }
        public string StartTime { get; set; }
        public string NextStopTime { get; set; }
        public string NextStopDistance { get; set; }
        public string ImageBottomDistance { get; set; }
    }

    public class Coordinate
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class StartPoint
    {
        public string Name { get; set; }
        public Coordinate Coordinate { get; set; }
        public string Time { get; set; }
        public string Id { get; set; }
    }

    public class Coordinate2
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class EndPoint
    {
        public string Name { get; set; }
        public Coordinate2 Coordinate { get; set; }
        public string Time { get; set; }
        public string Id { get; set; }
    }

    public class Transport
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public string Direction { get; set; }
        public string IconUrl { get; set; }
        public string ScheduleId { get; set; }
        public string TrackId { get; set; }
    }

    public class RouteSegment
    {
        public int RouteSegmentType { get; set; }
        public string IconUrl { get; set; }
        public int DurationMinutes { get; set; }
        public int WalkDistanceMeters { get; set; }
        public int DistanceMeters { get; set; }
        public int StopsCount { get; set; }
        public StartPoint StartPoint { get; set; }
        public EndPoint EndPoint { get; set; }
        public string Shape { get; set; }
        public Transport Transport { get; set; }
        public List<object> OtherTransports { get; set; }
    }

    public class Route
    {
        public string PreferenceLabel { get; set; }
        public int DurationMinutes { get; set; }
        public int WalkMinutes { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; }
        public List<RouteSegment> RouteSegments { get; set; }
    }

    public class RootObject
    {
        public List<Route> Routes { get; set; }
    }

    public class TransportTrafi
    {
        public RootObject HomeToAkro { get; set; }
        public RootObject HomeToGym { get; set; }
    }



}
