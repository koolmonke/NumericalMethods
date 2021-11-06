using System.Linq;
using MatrixArithmetic.Norms;
using static System.Math;

namespace MatrixArithmetic.Solvers
{
    public class SimpleIterationSolver : ISolver
    {
        public SimpleIterationSolver(INorma norma, Matrix matrix, Vector vector)
        {
            Norma = norma;
            System = matrix;
            FreeVector = vector;
            _tau = 2 / Norma.MatrixNorm(System);
        }

        public INorma Norma { get; }
        public Matrix System { get; }
        public Vector FreeVector { get; }

        private Vector? _solution;
        public Vector SolutionVector => _solution ??= Solve();

        public Vector Solve()
        {
            var guess = new GivensMethod(System, FreeVector).SolutionVector.Select(Truncate).ToVector();

            Vector xk = guess;
            Vector xkp;
            do
            {
                xkp = xk.Copy();
                xk = FreeVector.Sub(System.Multiply(xk.ToMatrix()).ToVector()).Multiply(_tau).Add(xk);
            } while (Norma.VectorNorm(xkp.Sub(xk)) > 1e-6);

            return xk;
        }

        public Vector Residual() =>
            System.Multiply(SolutionVector.ToMatrix()).ToVectorByColumn().Sub(FreeVector);

        private readonly double _tau;
    }
}