using System;
using System.Collections.Generic;

namespace Algo.Optim
{
    public abstract class SolutionInstance
    {
        private readonly SolutionSpace _space;
        private double _cost = -1.0;

        protected SolutionInstance(SolutionSpace space, int[] coord)
        {
            _space = space;
            Coordinates = coord;
        }

        public SolutionSpace Space => _space;

        public SolutionInstance FindBestAround()
        {
            return this;
        }

        public int[] Coordinates { get; }

        public double Cost => _cost >= 0 ? _cost : (_cost = ComputeCost());

        private double ComputeCost()
        {
            double c = DoComputeCost();
            if (_space.BestSolution == null || c < _space.BestSolution.Cost)
            {
                _space.BestSolution = this;
            }
            if (_space.WorstSolution == null || c > _space.WorstSolution.Cost)
            {
                _space.WorstSolution = this;
            }
            return c;
        }

        public IEnumerable<SolutionInstance> MonteCarloPath()
        {
            SolutionInstance last = this;
            for (;;)
            {
                yield return last;
                var best = last.BestAmongNeighbors;
                if (best == last) break;
                last = best;
            }
        }

        private SolutionInstance BestAmongNeighbors
        {
            get
            {
                SolutionInstance best = this;
                foreach (var n in Neighbors)
                {
                    if (n.Cost < best.Cost) best = n;
                }
                return best;
            }
        }

        private SolutionInstance RandomNeighbor
        {
            get
            {
                for (;;)
                {
                    var n = GetNeighbor(_space.Random.Next(_space.Dimension), _space.Random.Next(2) == 1);
                    if (n != null) return n;
                }
            }
        }

        private SolutionInstance GetNeighbor(int iDim, bool right)
        {
            int v = Coordinates[iDim];
            if (right)
            {
                if (++v >= _space.Cardinalities[iDim]) return null;
            }
            else
            {
                if (--v < 0) return null;
            }
            int[] prevCoords = (int[])Coordinates.Clone();
            prevCoords[iDim] = v;
            return _space.CreateSolutionInstance(prevCoords);
        }

        public SolutionInstance SimulatedAnnealing(int countPerTempDecrease = 100)
        {
            double temp = 1.0;
            double minTemp = 0.0001;
            double alpha = 0.9;
            SolutionInstance current = this;
            while (temp > minTemp)
            {
                for (int i = 0; i < countPerTempDecrease; ++i)
                {
                    var n = current.RandomNeighbor;
                    double deltaCost = current.Cost - n.Cost;
                    if (deltaCost >= 0 || Math.Exp(deltaCost / current.Cost / temp) > _space.Random.NextDouble())
                    {
                        current = n;
                    }
                }
                temp *= alpha;
            }
            return current;
        }

        public IEnumerable<SolutionInstance> Neighbors
        {
            get
            {
                for (int i = 0; i < _space.Dimension; ++i)
                {
                    int prevValue = Coordinates[i] - 1;
                    if (prevValue >= 0)
                    {
                        int[] prevCoords = (int[])Coordinates.Clone();
                        prevCoords[i] = prevValue;
                        yield return _space.CreateSolutionInstance(prevCoords);
                    }
                    int nextValue = Coordinates[i] + 1;
                    if (nextValue < _space.Cardinalities[i])
                    {
                        int[] nextCoords = (int[])Coordinates.Clone();
                        nextCoords[i] = nextValue;
                        yield return _space.CreateSolutionInstance(nextCoords);
                    }
                }
            }
        }

        protected abstract double DoComputeCost();
    }
}