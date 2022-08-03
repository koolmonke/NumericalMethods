using System.Collections.Generic;
using System.Linq;
using MatrixArithmetic;
using MatrixArithmetic.Norms;

namespace C1.Lab2
{
    public class JacobiSolver
    {
        public JacobiSolver(INorma norma, Matrix matrix, Vector vector)
        {
            Norma = norma;
            System = matrix;
            FreeVector = vector;
        }

        public Matrix System { get; }

        public Vector FreeVector { get; }

        public INorma Norma { get; }

        public Vector SolutionVector => _solutionVector ??= Solve();

        public Vector Residual =>
            System * SolutionVector - FreeVector;

        private Vector Solve()
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

                error = Norma.VectorNorm(x - x0);
                x0 = x.Copy();
            } while (error >= 1e-6);

            return x;
        }

        private Vector? _solutionVector;
    }
}