using System;
using System.Collections.Generic;
using System.Linq;

namespace MatrixArithmetic.Solvers
{
    public class RotationSolver : ISolver<double>
    {
        public RotationSolver(IMatrix<double> matrix, IVector<double> vector, double epsilon = Constants.Epsilon)
        {
            System = matrix;
            FreeVector = vector;
            LocalEpsilon = epsilon;
        }

        private double LocalEpsilon { get; }

        public IMatrix<double> System { get; }

        public IVector<double> FreeVector { get; }

        private IVector<double>? _solutionVector;

        public IVector<double> SolutionVector => _solutionVector ??= Solve();

        public IVector<double> Solve()
        {
            IVector<double> x0 = new Vector(System.N);
            IVector<double> x = new Vector(System.N);
            double error;

            do
            {
                for (int i = 0; i < System.N; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < System.M; j++)
                    {
                        if (i != j)
                        {
                            sum += System[i, j] * x0[j] / System[i, i];
                        }

                        x[i] = FreeVector[i] / System[i, i] - sum;
                    }
                }

                error = CalcError(x, x0);
                x0 = x.Copy();
            } while (error >= LocalEpsilon);

            return x.ToVector();
        }

        private static double CalcError(IEnumerable<double> a, IEnumerable<double> b) =>
            a.Zip(b)
                .Select(item => Math.Abs(item.First - item.Second))
                .Max();

        public IVector<double> Residual() =>
            System.Multiply(SolutionVector.ToMatrix()).ToVectorByColumn().Sub(FreeVector);
    }
}