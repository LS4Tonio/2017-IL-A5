using System;
using System.Collections.Generic;
using System.Linq;

namespace Algo.Optim
{
    public class Meeting : SolutionSpace
    {
        public int MaxFlightCount = 50;
        public double WaitingMinutePrice = 4;

        public Meeting(string flightDatabasePath, int seed)
            : base(seed)
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

            var cardinalities = new int[Guests.Count * 2];
            for (var i = 0; i < Guests.Count; i++)
            {
                GetPossibleArrivalFlights(Guests[i]);
                GetPossibleDepartureFlights(Guests[i]);

                cardinalities[i * 2] = Guests[i].ArrivalFlights.Count;
                cardinalities[i * 2 + 1] = Guests[i].DepartureFlights.Count;
            }
            Initialize(cardinalities);
        }

        public FlightDatabase Database { get; }
        public List<Guest> Guests { get; }
        public Airport Location { get; }
        public DateTime MaxArrivalDate { get; }
        public DateTime MinDepartureDate { get; }

        public double SolutionCardinality
        {
            get
            {
                return Guests.Select(g => (double)g.ArrivalFlights.Count * g.DepartureFlights.Count)
                             .Aggregate(1.0, (acc, card) => acc * card);
            }
        }

        protected override SolutionInstance CreateSolutionInstance(int[] coord)
        {
            return new MeetingInstance(this, coord);
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