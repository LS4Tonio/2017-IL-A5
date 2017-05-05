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
}