using System;
using MatrixArithmetic;
using MatrixArithmetic.Norms;

namespace Lab3
{
    public class SimpleIterationSolver
    {
        public SimpleIterationSolver(Func<Vector, double>[] vectorFunction, Vector guess, INorma norm)
        {
            VectorFunction = vectorFunction;
            Guess = guess;
            Norm = norm;
        }

        public Func<Vector, double>[] VectorFunction { get; }
        public Vector Guess { get; }
        public INorma Norm { get; }

        public int CounterIteration { get; private set; }

        public Vector SolutionVector => _solutionVector ??= Solve();
        
        private Vector? _solutionVector;

        private Vector Solve()
        {
            var xk = Guess;
            Vector xkp;
            do
            {
                xkp = xk;
                xk = VectorFunction.Apply(xk);
                CounterIteration++;
            } while (Norm.VectorNorm(xk - xkp) > 1e-3);

            return xk;
        }
    }
}