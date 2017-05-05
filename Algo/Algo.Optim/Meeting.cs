using System;
using System.Collections.Generic;

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
        }

        public FlightDatabase Database { get; }
        public DateTime MaxArrivalDate { get; }
        public DateTime MinDepartureDate { get; }
        public Airport Location { get; }
        public List<Guest> Guests { get; }
    }
}