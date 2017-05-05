using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algo.Optim
{
    public class MeetingInstance : SolutionInstance
    {
        public MeetingInstance(Meeting space, int[] coord)
            : base(space, coord)
        {
        }

        public new Meeting Space => (Meeting)base.Space;

        public DateTime BusTimeOnArrival { get; private set; }

        public DateTime BusTimeOnDeparture { get; private set; }

        protected override double DoComputeCost()
        {
            //return OurCompute();
            return SpiCompute();
        }

        private SimpleFlight ArrivalFlight(int guestIndex)
        {
            return Space.Guests[guestIndex].ArrivalFlights[guestIndex * 2];
        }

        private SimpleFlight DepartureFlight(int guestIndex)
        {
            return Space.Guests[guestIndex].DepartureFlights[guestIndex * 2 + 1];
        }

        private double SpiCompute()
        {
            var guests = Space.Guests.Select((g, idx) => new
            {
                Guest = g,
                Arrival = ArrivalFlight(idx),
                Departure = DepartureFlight(idx),
                Index = idx
            });
            var maxArrivalTime = BusTimeOnArrival = guests.Select(g => g.Arrival.ArrivalTime).Max();
            var minDepartureTime = BusTimeOnDeparture = guests.Select(g => g.Departure.DepartureTime).Min();

            var totalMinutesWaitArrival = guests.Select(g => (maxArrivalTime - g.Arrival.ArrivalTime).TotalMinutes)
                                                .Sum();
            var totalMinutesWaitDeparture = guests.Select(g => (g.Departure.DepartureTime - minDepartureTime).TotalMinutes)
                                                  .Sum();
            var waitCost = (totalMinutesWaitArrival + totalMinutesWaitDeparture) * Space.WaitingMinutePrice;

            var flightCost = guests.Select(g => g.Arrival.Price + g.Departure.Price).Sum();

            return waitCost + flightCost;
        }

        private double OurCompute()
        {
            var solutionCost = 0.0;
            for (var i = 0; i < Coordinates.Length; i += 2)
            {
                var user = Space.Guests[i / 2];

                var arrivalFlight = user.ArrivalFlights[Coordinates[i]];
                var departureFlight = user.DepartureFlights[Coordinates[i + 1]];

                var timeCostArrival = (Space.MaxArrivalDate - arrivalFlight.ArrivalTime).TotalMinutes * Space.WaitingMinutePrice;
                var timeCostDeparture = (departureFlight.DepartureTime - Space.MinDepartureDate).TotalMinutes * Space.WaitingMinutePrice;

                solutionCost += arrivalFlight.Price + timeCostArrival + departureFlight.Price + timeCostDeparture;
            }

            return solutionCost;
        }
    }
}