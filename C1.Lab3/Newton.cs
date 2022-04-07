using System;
using C1.Lab1;
using MatrixArithmetic;
using MatrixArithmetic.Norms;

namespace C1.Lab3
{
    public class Newton
    {
        public Newton(Func<Vector, double>[,] jacobiMatrix, Func<Vector, double>[] system, INorma norma, Vector guess)
        {
            JacobiMatrix = jacobiMatrix;
            System = system;
            Norma = norma;
            Guess = guess;
        }

        public Func<Vector, double>[,] JacobiMatrix { get; }

        public Func<Vector, double>[] System { get; }

        public INorma Norma { get; }
        public Vector Guess { get; }

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
                var jac = JacobiMatrix.Apply(xk);
                var f = System.Apply(xk);
                var gauss = new GaussSolver(jac, -f).SolutionVector;
                xk += gauss;
                CounterIteration++;
            } while (Norma.VectorNorm(xk - xkp) > Constants.Epsilon6);

            return xk;
        }
    }
}