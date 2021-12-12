using System;
using System.Collections.Generic;
using System.Linq;

namespace MatrixArithmetic
{
    public static class FuncExtensionMethods
    {
        public static Matrix Apply(this Func<Vector, double>[,] matrixFunc, Vector vector)
        {
            var n = matrixFunc.GetLength(0);
            var m = matrixFunc.GetLength(1);
            var result = new Matrix(n, m);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    result[i, j] = matrixFunc[i, j](vector);
                }
            }

            return result;
        }

        public static Vector Apply(this IEnumerable<Func<Vector, double>> vectorFunc, Vector vector) =>
            vectorFunc.Select(f => f(vector)).ToVector();
    }
}