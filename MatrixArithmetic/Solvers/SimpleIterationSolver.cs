using System;
using System.Linq;
using MatrixArithmetic.Norms;
using static System.Math;

namespace MatrixArithmetic.Solvers
{
    public class SimpleIterationSolver : ISolver<double>
    {
        private double _tau;

        public SimpleIterationSolver(INorma norma, IMatrix<double> matrix, IVector<double> vector)
        {
            Norma = norma;
            Matrix = matrix;
            Vector = vector;
        }

        public INorma Norma { get; }
        public IMatrix<double> Matrix { get; }
        public IVector<double> Vector { get; }

        private IVector<double>? _solution;
        public IVector<double> SolutionVector => _solution ??= Solve();

        public IVector<double> Solve()
        {
            _tau = 2 / Norma.MatrixNorm(Matrix);
            var guess = new RotationSolver(Matrix, Vector).SolutionVector.Select(Truncate).ToVector();
            var solveFor = Vector;

            var xk = guess.Copy();
            Vector xkp;
            do
            {
                xkp = xk.ToVector();
                xk = solveFor.Sub(Matrix.Multiply(xk.ToMatrix()).ToVector()).Select(item => _tau * item).ToVector()
                    .Add(xk);
            } while (Norma.VectorNorm(xkp.Sub(xk)) > 1e-5);

            return xk;
        }

        public IVector<double> Residual() => Matrix.Multiply(SolutionVector.ToMatrix()).ToVectorByColumn().Sub(Vector);
    }
}