using System;
using System.Linq;
using MatrixArithmetic.Norms;
using static System.Math;

namespace MatrixArithmetic.Solvers
{
    public class SimpleIterationSolver : ISolver<double>
    {
        public SimpleIterationSolver(INorma norma, IMatrix<double> matrix, IVector<double> vector)
        {
            Norma = norma;
            System = matrix;
            FreeVector = vector;
            _tau = 2 / Norma.MatrixNorm(System);
        }

        public INorma Norma { get; }
        public IMatrix<double> System { get; }
        public IVector<double> FreeVector { get; }

        private IVector<double>? _solution;
        public IVector<double> SolutionVector => _solution ??= Solve();

        public IVector<double> Solve()
        {
            var guess = new RotationSolver(System, FreeVector, 0.1).SolutionVector;

            IVector<double> xk = guess;
            IVector<double> xkp;
            do
            {
                xkp = xk.Copy();
                xk = FreeVector.Sub(System.Multiply(xk.ToMatrix()).ToVector()).Multiply(_tau).Add(xk);
            } while (Norma.VectorNorm(xkp.Sub(xk)) > 1e-6);

            return xk;
        }

        public IVector<double> Residual() =>
            System.Multiply(SolutionVector.ToMatrix()).ToVectorByColumn().Sub(FreeVector);

        private readonly double _tau;
    }
}