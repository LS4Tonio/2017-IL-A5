using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algo.Optim
{
    public class MeetingInstance : SolutionInstance
    {
        private readonly Meeting _meeting;
        private const int MinuteCost = 1;

        public MeetingInstance(SolutionSpace space, int[] coord)
            : base(space, coord)
        {
            _meeting = space as Meeting;
        }

        protected override double DoComputeCost()
        {
            var solutionCost = 0.0;

            for (var i = 0; i < Coordinates.Length; i += 2)
            {
                var user = _meeting.Guests[i / 2];

                var arrivalFlight = user.ArrivalFlights[Coordinates[i]];
                var departureFlight = user.DepartureFlights[Coordinates[i + 1]];

                var timeCostArrival = (_meeting.MaxArrivalDate - arrivalFlight.ArrivalTime).Minutes * MinuteCost;
                var timeCostDeparture = (departureFlight.DepartureTime - _meeting.MinDepartureDate).Minutes * MinuteCost;

                solutionCost += arrivalFlight.Price + timeCostArrival + departureFlight.Price + timeCostDeparture;
            }

            return solutionCost;
        }
    }
}