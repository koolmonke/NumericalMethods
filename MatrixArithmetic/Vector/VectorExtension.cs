using System.Collections.Generic;
using System.Linq;

namespace MatrixArithmetic
{
    public static class VectorExt
    {
        public static Vector ToVector(this IEnumerable<double> enumerable) => new Vector(enumerable.ToArray());

        internal static IMatrix<double> ToMatrix(this IVector<double>[] vectors)
        {
            var firstVectorN = vectors[0].N;
            if (vectors.Skip(1).Any(vector => vector.N != firstVectorN))
            {
                throw new VectorDifferentDimException();
            }

            var result = new Matrix(firstVectorN, vectors.Length);

            for (int i = 0; i < vectors.Length; i++)
            {
                for (int j = 0; j < firstVectorN; j++)
                {
                    result[j, i] = vectors[i][j];
                }
            }

            return result;
        }

        public static Matrix ToMatrix(this IVector<double> vector)
        {
            var n = vector.N;
            var result = new Matrix(n, 1);

            for (int i = 0; i < n; i++)
            {
                result[i, 0] = vector[i];
            }

            return result;
        }
    }
}