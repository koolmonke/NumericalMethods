using System.Collections.Generic;
using System.Linq;
using MatrixArithmetic.Norms;

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

        public Vector Residual =>
            System.Multiply(SolutionVector).Sub(FreeVector);

        private double LocalEpsilon { get; }

        private static double CalcError(IEnumerable<double> a, IEnumerable<double> b)
        {
            return new TaxiCabNorm().VectorNorm(a.Zip(b)
                .Select(item => item.First - item.Second).ToVector());
        }
    }
}