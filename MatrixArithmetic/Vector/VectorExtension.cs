using System.Collections.Generic;
using System.Linq;

namespace MatrixArithmetic
{
    public static class VectorExt
    {
        public static Vector ToVector(this IEnumerable<double> enumerable) => new Vector(enumerable.ToArray());
    }
}