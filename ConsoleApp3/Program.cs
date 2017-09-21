using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Flight> flights = LoadFlights(); //Magic method which loads all the flights between the airports

            var fastestRoute = FindFastestRoute(flights, "ATL", "SAN");

            var route = String.Join(",", fastestRoute.Select(x => String.Format("{0}->{1}", x.FromAirportCode, x.ToAirportCode)));
            var time = fastestRoute.Sum(x => x.FlightDuration);

            Assert.AreEqual(route, "ATL->COS,COS->OAK,OAK->SAN");
            Assert.AreEqual(time, 8.9f);

        }

        public class Flight
        {
            public string FlightNumber { get; set; }
            public string FromAirportCode { get; set; }
            public string ToAirportCode { get; set; }
            public float Distance { get; set; }
            public float FlightDuration { get; set; }
        }

        public static List<Flight> FindFastestRoute(List<Flight> flights, string from, string to)
        {
            var mapFligth = new MapFlight();
            flights.ForEach(f =>
            {
                mapFligth.AddConnection(f.FromAirportCode, f);
            });

           
            var routesFromOrigin = new List<List<Flight>>();
            if (mapFligth.connections.ContainsKey(from))
            {
                mapFligth.connections[from].ForEach(x =>
                {
                    var alreadyFlew = new List<string>();
                    var flightsToTake = new List<Flight>();
                    alreadyFlew.Add(from);
                    routesFromOrigin.Add(possibleRoute(mapFligth, alreadyFlew, flightsToTake, x, to));
                });
            }

            var routesUntilDestination = new List<List<Flight>>();
            foreach (var route in routesFromOrigin.Where(rs => rs.Any(f => f.ToAirportCode == to)))
            {
                routesUntilDestination.Add(route.GetRange(0, route.IndexOf(route.First(f => f.ToAirportCode == to)) + 1));
            }
            return routesUntilDestination.OrderBy(c => c.Count).FirstOrDefault() ?? new List<Flight>();
        }

        private static List<Flight> possibleRoute(MapFlight map, List<string> alreadyFlew, List<Flight> take, Flight connection, string to)
        {
            alreadyFlew.Add(connection.ToAirportCode);
            take.Add(connection);
            if (connection.ToAirportCode == to)
            {
                return take;
            }
            else
            {
                if (map.connections.ContainsKey(connection.ToAirportCode))
                {
                    map.connections[connection.ToAirportCode].ForEach(c =>
                    {
                        if (!alreadyFlew.Contains(c.ToAirportCode))
                        {
                            possibleRoute(map, alreadyFlew, take, c, to);
                        }
                    });
                }
            }
            return take;

        }
        
        public class MapFlight {
            public Dictionary<string, List<Flight>> connections { get; set; }
            public MapFlight()
            {
                connections = new Dictionary<string, List<Flight>>();
            }
            public void AddConnection (string s, Flight f)
            {
                if (!connections.ContainsKey(s))
                    connections[s] = new List<Flight>();
                connections[s].Add(f);
            }
        }

        public static List<Flight> LoadFlights()
        {
            return new List<Flight>()
        {
            new Flight() { FlightNumber = "001", FromAirportCode= "ATL",  ToAirportCode= "ORD", Distance= 1000, FlightDuration= 2.5f },

            new Flight() { FlightNumber= "002", FromAirportCode= "OAK",  ToAirportCode= "SAN", Distance= 1100, FlightDuration= 2.6f },

            new Flight() { FlightNumber= "003", FromAirportCode= "OAK",  ToAirportCode= "ATL", Distance= 1100, FlightDuration= 2.6f },

            new Flight() { FlightNumber= "004", FromAirportCode= "ATL",  ToAirportCode= "MDW", Distance= 1200, FlightDuration= 2.7f },
            new Flight() { FlightNumber= "005", FromAirportCode= "MDW",  ToAirportCode= "ORD", Distance= 1300, FlightDuration= 2.8f },

            new Flight() { FlightNumber= "006", FromAirportCode= "BOI",  ToAirportCode= "MDW", Distance= 1400, FlightDuration= 2.9f },

            new Flight() { FlightNumber= "007", FromAirportCode= "ORD",  ToAirportCode= "ATL", Distance= 1500, FlightDuration= 3.0f },

            new Flight() { FlightNumber= "008", FromAirportCode= "ATL",  ToAirportCode= "COS", Distance= 1600, FlightDuration= 3.1f },

            new Flight() { FlightNumber= "009", FromAirportCode= "COS",  ToAirportCode= "OAK", Distance= 1700, FlightDuration= 3.2f },

            new Flight() { FlightNumber= "010", FromAirportCode= "COS",  ToAirportCode= "BOI", Distance= 1800, FlightDuration= 3.3f }

            };

        }       

    }
}
