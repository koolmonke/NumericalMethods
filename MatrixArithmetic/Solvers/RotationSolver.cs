using System;
using System.Collections.Generic;
using System.Linq;

namespace MatrixArithmetic.Solvers
{
    public class RotationSolver : ISolver<double>
    {
        public RotationSolver(IMatrix<double> matrix, IVector<double> vector)
        {
            Matrix = matrix;
            Vector = vector;
        }

        public IMatrix<double> Matrix { get; }

        public IVector<double> Vector { get; }

        private IVector<double>? _solutionVector;

        public IVector<double> SolutionVector => _solutionVector ??= Solve();

        public IVector<double> Solve()
        {
            double[] x0 = new double[Matrix.N];
            double[] x = (double[])x0.Clone();
            double erro = double.MaxValue;


            while (erro >= Constants.Epsilon)
            {
                for (int i = 0; i < Matrix.N; i++)
                {
                    double soma = 0;
                    for (int j = 0; j < Matrix.M; j++)
                    {
                        if (i != j)
                        {
                            soma += Matrix[i, j] * x0[j] / Matrix[i, i];
                        }

                        x[i] = Vector[i] / Matrix[i, i] - soma;
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

        public IVector<double> Residual() => Matrix.Multiply(SolutionVector.ToMatrix()).ToVectorByColumn().Sub(Vector);
    }
}