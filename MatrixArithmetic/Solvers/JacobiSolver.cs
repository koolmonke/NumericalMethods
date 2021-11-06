using System;
using System.Collections.Generic;
using System.Linq;

namespace MatrixArithmetic.Solvers
{
    public class JacobiSolver : ISolver
    {
        public JacobiSolver(Matrix matrix, Vector vector, double epsilon = Constants.Epsilon)
        {
            System = matrix;
            FreeVector = vector;
            LocalEpsilon = epsilon;
        }

        private double LocalEpsilon { get; }

        public Matrix System { get; }

        public Vector FreeVector { get; }

        private Vector? _solutionVector;

        public Vector SolutionVector => _solutionVector ??= Solve();

        public Vector Solve()
        {
            Vector x0 = new Vector(System.N);
            Vector x = new Vector(System.N);
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

        public Vector Residual() =>
            System.Multiply(SolutionVector.ToMatrix()).ToVectorByColumn().Sub(FreeVector);
    }
}