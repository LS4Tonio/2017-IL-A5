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

        public int Dimension => Cardinalities.Length;
        public int[] Cardinalities { get; private set; }
        public SolutionInstance BestSolution { get; internal set; }

        public SolutionInstance GetRandomInstance()
        {
            var coor = new int[Dimension];
            for (int i = 0; i < coor.Length; ++i)
            {
                coor[i] = _random.Next(Cardinalities[i]);
            }

            return CreateSolutionInstance(coor);
        }

        public void TryRandom(int nbTry)
        {
            while (--nbTry >=)
            {
                var c = GetRandomInstance().Cost;
            }
        }

        protected abstract SolutionInstance CreateSolutionInstance(int[] coord);
    }
}