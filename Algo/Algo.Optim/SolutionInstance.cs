namespace Algo.Optim
{
    public abstract class SolutionInstance
    {
        private readonly SolutionSpace _space;
        private double _cost = -1;

        public SolutionInstance(SolutionSpace space, int[] coord)
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
            var c = DoComputeCost();
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

        protected abstract double DoComputeCost();
    }
}