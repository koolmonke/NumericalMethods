using System.Collections.Generic;
using System.Linq;
using static System.Math;
namespace MatrixArithmetic.Norms
{
    public class TaxiCabNorm : INorma
    {
        public double VectorNorm(Vector vector) => vector.Sum(Abs);

        public double MatrixNorm(Matrix matrix)
        {
            IEnumerable<double> ColumnNorm(Matrix columns)
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