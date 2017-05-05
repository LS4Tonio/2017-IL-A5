using System;
using System.Collections.Generic;
using System.Linq;

namespace Algo.Optim
{
    public class Guest
    {
        public Guest()
        {
            ArrivalFlights = new List<SimpleFlight>();
            DepartureFlights = new List<SimpleFlight>();
        }

        public string Name { get; set; }

        public Airport Location { get; set; }

        public List<SimpleFlight> ArrivalFlights { get; }

        public List<SimpleFlight> DepartureFlights { get; }

    }

    public class Meeting
    {
        public Meeting(string flightDatabasePath)
        {
            Database = new FlightDatabase(flightDatabasePath);
            Location = Airport.FindByCode("LHR");
            Guests = new List<Guest>
            {
                new Guest
                {
                    Name = "Rudolphe",
                    Location = Airport.FindByCode("BER")
                },
                new Guest
                {
                    Name = "Adeline",
                    Location = Airport.FindByCode("CDG")
                },
                new Guest
                {
                    Name = "Marcel",
                    Location = Airport.FindByCode("MRS")
                },
                new Guest
                {
                    Name = "Léon",
                    Location = Airport.FindByCode("LYS")
                },
                new Guest
                {
                    Name = "Peter",
                    Location = Airport.FindByCode("MAN")
                },
                new Guest
                {
                    Name = "Jose",
                    Location = Airport.FindByCode("BIO")
                },
                new Guest
                {
                    Name = "Barrack",
                    Location = Airport.FindByCode("JFK")
                },
                new Guest
                {
                    Name = "Youssef",
                    Location = Airport.FindByCode("TUN")
                },
                new Guest
                {
                    Name = "Mario",
                    Location = Airport.FindByCode("MXP")
                }
            };
            MaxArrivalDate = new DateTime(2010, 7, 27, 17, 0, 0);
            MinDepartureDate = new DateTime(2010, 8, 3, 15, 0, 0);
            foreach (var g in Guests)
            {
                SelectCandidateFlightsForArrival(g);
                SelectCandidateFlightsForDeparture(g);
            }

            foreach (var guest in Guests)
            {
                GetPossibleArrivalFlights(guest);
                GetPossibleDepartureFlights(guest);
            }
        }
                            .Take(MaxFlightCount);
            g.ArrivalFlights.AddRange(flights);
        }

        void SelectCandidateFlightsForDeparture(Guest g)
        {
            var flights = Database.GetFlights(MinDepartureDate, Location, g.Location)
                                    .Where(f => f.DepartureTime > MinDepartureDate)
                                    .OrderBy(f => f.DepartureTime)
                                    .Take(MaxFlightCount);
            g.DepartureFlights.AddRange(flights);
        }

        public double SolutionCardinality => Guests.Select(g => (double)g.ArrivalFlights.Count * g.DepartureFlights.Count)
                                                    .Aggregate(1.0, (acc, card) => acc * card);

        public FlightDatabase Database { get; }
        public DateTime MaxArrivalDate { get; }
        public DateTime MinDepartureDate { get; }
        public Airport Location { get; }
        public List<Guest> Guests { get; }
        public int MaxFlightCount = 50;

        public double SolutionCardinality
        {
            get
            {
                return Guests.Select(g => (double)g.ArrivalFlights.Count * g.DepartureFlights.Count)
                             .Aggregate(1.0, (acc, card) => acc * card);
            }
        }

        private void GetPossibleArrivalFlights(Guest guest)
        {
            var flights = Database.GetFlights(MaxArrivalDate.Date.AddDays(-1), guest.Location, Location)
                                  .Concat(Database.GetFlights(MaxArrivalDate.Date, guest.Location, Location))
                                  .Where(x => x.ArrivalTime.TimeOfDay >= MaxArrivalDate.TimeOfDay.Subtract(TimeSpan.FromHours(5))
                                              && x.ArrivalTime.TimeOfDay < MaxArrivalDate.TimeOfDay)
                                  .OrderByDescending(x => x.ArrivalTime)
                                  .Take(MaxFlightCount)
                                  .ToList();

            guest.ArrivalFlights.AddRange(flights);
        }

        private void GetPossibleDepartureFlights(Guest guest)
        {
            var flights = Database.GetFlights(MinDepartureDate, Location, guest.Location)
                                  .Where(x => x.DepartureTime.TimeOfDay > MinDepartureDate.TimeOfDay
                                              && x.DepartureTime.TimeOfDay <= MinDepartureDate.TimeOfDay.Add(TimeSpan.FromHours(5)))
                                  .OrderBy(x => x.DepartureTime)
                                  .Take(MaxFlightCount)
                                  .ToList();

            guest.DepartureFlights.AddRange(flights);
        }
    }
}