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

        protected SolutionSpace(int seed)
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

        public SolutionInstance GetRandomInstance()
        {
            int[] coord = new int[Dimension];
            for (int i = 0; i < Dimension; ++i)
            {
                coord[i] = _random.Next(Cardinalities[i]);
            }
            return CreateSolutionInstance(coord);
        }

        public void TryRandom(int nbTry)
        {
            while (--nbTry >= 0)
            {
                var c = GetRandomInstance().Cost;
            }
        }

        internal protected abstract SolutionInstance CreateSolutionInstance(int[] coord);

        public SolutionInstance RecuitSimule()
        {
            SolutionInstance best = this.GetRandomInstance();
            double T_min = 0.00001;
            double T = 1;
            var old_cost = best.Cost;
            double alpha = 0.9;
            var random = new Random();

            while (T > T_min)
            {
                for (int i = 1; i <= 100; i++)
                {
                    SolutionInstance intance = this.GetRandomInstance();
                    if (best != intance)
                    {
                        foreach (var n in intance.Neighbors)
                        {
                            var new_cost = n.Cost;
                            var ap = Math.Exp((old_cost - new_cost) / T);
                            if (ap > random.Next())
                            {
                                best = n;
                                old_cost = new_cost;
                            }
                        }
                    }
                }
                T = T * alpha;
            }

            return best;
        }
    }
}
