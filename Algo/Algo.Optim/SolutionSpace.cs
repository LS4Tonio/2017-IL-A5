using System;

namespace Algo.Optim
{
    public abstract class SolutionSpace
    {
        private readonly Random _random;

        protected SolutionSpace(int seed)
        {
            _random = new Random(seed);
        }

        public SolutionInstance BestSolution { get; internal set; }
        public SolutionInstance WorstSolution { get; internal set; }
        public int[] Cardinalities { get; private set; }
        public int Dimension => Cardinalities.Length;

        public SolutionInstance GetRandomInstance()
        {
            var coor = new int[Dimension];
            for (var i = 0; i < Dimension; ++i)
            {
                coor[i] = _random.Next(Cardinalities[i]);
            }
            return CreateSolutionInstance(coor);
        }

        public void TryRandom(int nbTry)
        {
            while (--nbTry >= 0)
            {
                var c = GetRandomInstance().Cost;
            }
        }

        protected abstract SolutionInstance CreateSolutionInstance(int[] coord);

        protected void Initialize(int[] cardinalities)
        {
            Cardinalities = cardinalities;
        }
    }
}