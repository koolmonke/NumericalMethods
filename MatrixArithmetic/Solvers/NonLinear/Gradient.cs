using System;
using MatrixArithmetic.Norms;
using MatrixArithmetic.Solvers.Gauss;

namespace MatrixArithmetic.Solvers.NonLinear
{
    public class Gradient
    {
        public Gradient(Func<Vector, double>[,] jacobiMatrix, Func<Vector, double>[] system, INorma norma, Vector guess)
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

        private Vector? _solutionVector;
        public Vector SolutionVector => _solutionVector ??= Solve();

        private Vector Solve()
        {
            var xk = Guess;

            Vector xkp;
            do
            {
                xkp = xk;
                var jac = JacobiMatrix.Apply(xk);
                var f = System.Apply(xk);
                var transpose = jac.Transpose() * f;
                xk -= NextTau(jac, f) * transpose;
                CounterIteration++;
            } while (Norma.VectorNorm(xk - xkp) > 1e-3);

            return xk;
        }

        private static double NextTau(Matrix jac, Vector f)
        {
            var temp = jac * jac.Transpose() * f;
            return temp * f / (temp * temp);
        }
    }
}