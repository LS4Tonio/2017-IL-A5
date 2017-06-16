using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algo.Optim
{
    public abstract class SolutionSpace
    {
        readonly Random _random;

        protected SolutionSpace(int seed )
        {
            _random = new Random(seed);
        }

        public SolutionInstance BestSolution { get; internal set; }

        public SolutionInstance WorstSolution { get; internal set; }

        protected void Initialize(int[] cardinalities)
        {
            Cardinalities = cardinalities;
        }

        public int Dimension => Cardinalities.Length;

        public int[] Cardinalities { get; private set; }

        public Random Random => _random;

        public SolutionInstance GetRandomInstance()
        {
            int[] coord = new int[Dimension];
            for(int i = 0; i < Dimension; ++i )
            {
                coord[i] = Random.Next(Cardinalities[i]);
            }
            return CreateSolutionInstance(coord);
        }

        public void TryRandom(int nbTry, bool useMonteCarlo = false)
        {
            double forceComputeCost;
            while (--nbTry >= 0)
            {
                var r = GetRandomInstance();
                if (useMonteCarlo) r = r.MonteCarloPath().Last();
                else forceComputeCost = r.Cost;
            }
        }
        public Tuple<int,int,int> TrySimulatedAnnealing(int nbTry, int countPerTempDecrease)
        {
            int aWin = 0;
            int amWin = 0;
            int maWin = 0;
            while (--nbTry >= 0)
            {
                var r = GetRandomInstance();
                var a = r.SimulatedAnnealing( countPerTempDecrease );
                var m = r.MonteCarloPath().Last();
                var ma = m.SimulatedAnnealing(countPerTempDecrease);
                var am = a.MonteCarloPath().Last();
                if (a.Cost < m.Cost) ++aWin;
                if (am.Cost < a.Cost) ++amWin;
                if (ma.Cost < a.Cost) ++maWin;
            }
            return Tuple.Create( aWin, amWin, maWin);
        }

        internal protected abstract SolutionInstance CreateSolutionInstance(int[] coord);
    }
}
