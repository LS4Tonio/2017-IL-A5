﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Algo.Optim
{
    public class Guest
    {
        public string Name { get; set; }

        public Airport Location { get; set; }

        public List<SimpleFlight> ArrivalFlights { get; } = new List<SimpleFlight>();

        public List<SimpleFlight> DepartureFlights { get; } = new List<SimpleFlight>();
    }

    public class Meeting : SolutionSpace
    {
        public Meeting(string flightDatabasePath, int randomSeed)
            : base(randomSeed)
        {
            Database = new FlightDatabase(flightDatabasePath);
            Location = Airport.FindByCode("LHR");
            Guests.Add(new Guest()
            {
                Name = "Adolf",
                Location = Airport.FindByCode("BER")
            });
            Guests.Add(new Guest()
            {
                Name = "Adeline",
                Location = Airport.FindByCode("CDG")
            });
            Guests.Add(new Guest()
            {
                Name = "Marcel",
                Location = Airport.FindByCode("MRS")
            });
            Guests.Add(new Guest()
            {
                Name = "Léon",
                Location = Airport.FindByCode("LYS")
            });
            Guests.Add(new Guest()
            {
                Name = "Peter",
                Location = Airport.FindByCode("MAN")
            });
            Guests.Add(new Guest()
            {
                Name = "Jose",
                Location = Airport.FindByCode("BIO")
            });
            Guests.Add(new Guest()
            {
                Name = "Donald",
                Location = Airport.FindByCode("JFK")
            });
            Guests.Add(new Guest()
            {
                Name = "Youssef",
                Location = Airport.FindByCode("TUN")
            });
            Guests.Add(new Guest()
            {
                Name = "Mario",
                Location = Airport.FindByCode("MXP")
            });
            MaxArrivalDate = new DateTime(2010, 7, 27, 17, 0, 0);
            MinDepartureDate = new DateTime(2010, 8, 3, 15, 0, 0);
            foreach (var g in Guests)
            {
                SelectCandidateFlightsForArrival(g);
                SelectCandidateFlightsForDeparture(g);
            }
            Initialize(Guests.Select(g => new { A = g.ArrivalFlights.Count, D = g.DepartureFlights.Count })
                                .Aggregate(new List<int>(), (list, e) =>
                                               {
                                                   list.Add(e.A);
                                                   list.Add(e.D);
                                                   return list;
                                               })
                                .ToArray());
        }

        private void SelectCandidateFlightsForArrival(Guest g)
        {
            var flights = Database.GetFlights(MaxArrivalDate, g.Location, Location)
                            .Concat(Database.GetFlights(MaxArrivalDate.AddDays(-1), g.Location, Location))
                            .Where(f => f.ArrivalTime < MaxArrivalDate)
                            .OrderByDescending(f => f.ArrivalTime)
                            .Take(MaxFlightCount);
            g.ArrivalFlights.AddRange(flights);
        }

        private void SelectCandidateFlightsForDeparture(Guest g)
        {
            var flights = Database.GetFlights(MinDepartureDate, Location, g.Location)
                                    .Where(f => f.DepartureTime > MinDepartureDate)
                                    .OrderBy(f => f.DepartureTime)
                                    .Take(MaxFlightCount);
            g.DepartureFlights.AddRange(flights);
        }

        protected internal override SolutionInstance CreateSolutionInstance(int[] coord)
        {
            return new MeetingInstance(this, coord);
        }

        public double SolutionCardinality => Guests.Select(g => (double)g.ArrivalFlights.Count * g.DepartureFlights.Count)
                                                    .Aggregate(1.0, (acc, card) => acc * card);

        public FlightDatabase Database { get; }

        public List<Guest> Guests { get; } = new List<Guest>();

        public int MaxFlightCount = 50;

        public double WaitingMinutePrice(double count)
        {
            if (count < 10) return 0;
            if (count < 20) return count * 4;
            if (count < 40) return (count - 20) * 8 + 4 * 20;
            if (count < 60) return (count - 40) * 10 + ((40 - 20) * 8 + 4 * 20);
            return (count - 60) * 15 + (count - 60) * 10 + ((40 - 20) * 8 + 4 * 20);
        }

        public DateTime MaxArrivalDate { get; }

        public DateTime MinDepartureDate { get; }

        public Airport Location { get; private set; }
    }
}