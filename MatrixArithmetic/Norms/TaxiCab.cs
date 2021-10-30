using System.Collections.Generic;
using System.Linq;
using static System.Math;

namespace MatrixArithmetic.Norms
{
    public class TaxiCabNorm : INorma
    {
        public double VectorNorm(IVector<double> vector) => vector.Sum(Abs);

        public double MatrixNorm(IMatrix<double> matrix)
        {
            IEnumerable<double> ColumnNorm(IMatrix<double> columns)
            {
                for (var i = 0; i < columns.N; i++)
                {
                    yield return VectorNorm(columns.GetColumn(i));
                }
            }

            return ColumnNorm(matrix).Max();
        }
    }
}