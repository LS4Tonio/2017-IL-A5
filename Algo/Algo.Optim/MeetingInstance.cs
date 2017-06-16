﻿using System;
using System.Linq;

namespace Algo.Optim
{
    public class MeetingInstance : SolutionInstance
    {
        public MeetingInstance(Meeting m, int[] coord)
            : base(m, coord)
        {
        }

        public new Meeting Space => (Meeting)base.Space;

        public DateTime BusTimeOnArrival { get; private set; }

        public DateTime BusTimeOnDeparture { get; private set; }

        private SimpleFlight ArrivalFor(int guestIdx)
        {
            return Space.Guests[guestIdx].ArrivalFlights[Coordinates[guestIdx * 2]];
        }

        private SimpleFlight DepartureFor(int guestIdx)
        {
            return Space.Guests[guestIdx].DepartureFlights[Coordinates[guestIdx * 2 + 1]];
        }

        protected override double DoComputeCost()
        {
            var guests = Space.Guests.Select((g, idx) => new
            {
                Guest = g,
                Arrival = ArrivalFor(idx),
                Departure = DepartureFor(idx),
                Index = idx
            });
            var maxArrivalTime = BusTimeOnArrival = guests.Select(g => g.Arrival.ArrivalTime).Max();
            var minDepartureTime = BusTimeOnDeparture = guests.Select(g => g.Departure.DepartureTime).Min();

            var totalMinutesWaitArrival = guests.Select(g => (maxArrivalTime - g.Arrival.ArrivalTime).TotalMinutes)
                                                .Sum();
            var totalMinutesWaitDeparture = guests.Select(g => (g.Departure.DepartureTime - minDepartureTime).TotalMinutes)
                                                .Sum();
            var waitCost = Space.WaitingMinutePrice(totalMinutesWaitArrival + totalMinutesWaitDeparture);

            var flightCost = guests.Select(g => g.Arrival.Price + g.Departure.Price).Sum();

            return waitCost + flightCost;
        }
    }
}