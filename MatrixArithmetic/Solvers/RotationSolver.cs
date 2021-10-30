using System;
using System.Collections.Generic;
using System.Linq;

namespace MatrixArithmetic.Solvers
{
    public class RotationSolver : ISolver<double>
    {
        public RotationSolver(IMatrix<double> matrix, IVector<double> vector)
        {
            System = matrix;
            FreeVector = vector;
        }

        public IMatrix<double> System { get; }

        public IVector<double> FreeVector { get; }

        private IVector<double>? _solutionVector;

        public IVector<double> SolutionVector => _solutionVector ??= Solve();

        public IVector<double> Solve()
        {
            double[] x0 = new double[System.N];
            double[] x = (double[])x0.Clone();
            double erro = double.MaxValue;


            while (erro >= Constants.Epsilon)
            {
                for (int i = 0; i < System.N; i++)
                {
                    double soma = 0;
                    for (int j = 0; j < System.M; j++)
                    {
                        if (i != j)
                        {
                            soma += System[i, j] * x0[j] / System[i, i];
                        }

                        x[i] = FreeVector[i] / System[i, i] - soma;
                    }
                }

                erro = CalcErro(x, x0);
                x0 = (double[])x.Clone();
            }

            return x.ToVector();
        }

        private static double CalcErro(IEnumerable<double> a, IEnumerable<double> b) =>
            a.Zip(b)
                .Select(item => Math.Abs(item.First - item.Second))
                .Max();

        public IVector<double> Residual() => System.Multiply(SolutionVector.ToMatrix()).ToVectorByColumn().Sub(FreeVector);
    }
}