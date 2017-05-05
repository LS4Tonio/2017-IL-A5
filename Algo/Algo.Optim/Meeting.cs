﻿using System;
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

            foreach (var guest in Guests)
            {
                GetPossibleArrivalFlights(guest);
                GetPossibleDepartureFlights(guest);
            }
        }

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
            }
        }

        private void GetPossibleArrivalFlights(Guest guest)
        {
            var flightsDay = Database.GetFlights(MaxArrivalDate.Date, guest.Location, Location);
            var flightsDayBefore = Database.GetFlights(MaxArrivalDate.Date.AddDays(-1), guest.Location, Location);
            flightsDayBefore.ToList().AddRange(flightsDay);

            var flights = flightsDayBefore
                .Where(x => x.ArrivalTime.TimeOfDay >= MaxArrivalDate.TimeOfDay.Subtract(TimeSpan.FromHours(5))
                            && x.ArrivalTime.TimeOfDay < MaxArrivalDate.TimeOfDay)
                .OrderByDescending(x => x.ArrivalTime)
                .Take(50)
                .ToList();

            guest.ArrivalFlights.AddRange(flights);
        }

        private void GetPossibleDepartureFlights(Guest guest)
        {
            var flights = Database.GetFlights(MinDepartureDate, Location, guest.Location)
                           .Where(x => x.DepartureTime.TimeOfDay > MinDepartureDate.TimeOfDay
                                       /*&& x.DepartureTime.TimeOfDay <= MinDepartureDate.TimeOfDay.Add(TimeSpan.FromHours(5))*/)
                           .OrderBy(x => x.DepartureTime)
                           .Take(50)
                           .ToList();

            guest.DepartureFlights.AddRange(flights);
        }
    }
}