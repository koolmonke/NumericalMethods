using System.Collections.Generic;

namespace MatrixArithmetic
{
    public interface IMatrix<T> : IEnumerable<T>
    {
        int N { get; }
        int M { get; }

        T[,] ToRepresentation();

        T this[int i, int j] { get; set; }

        IMatrix<T> From(IEnumerable<T> values);

        IVector<T> ToVectorByColumn(int column = 0);
        IVector<T> ToVectorByRow(int row = 0);

        IMatrix<T> Multiply(IMatrix<T> right);

        public void SwitchRows(int row1, int row2)
        {
            if (row1 == row2)
                return;

            for (int col = 0; col < M; col++)
            {
                (this[row1, col], this[row2, col]) = (this[row2, col], this[row1, col]);
            }
        }

        IMatrix<double> ConcatHorizontally(IMatrix<double> other);

        IMatrix<double> ExtractColumns(int[] cols);

        IMatrix<double> ExtractColumns(int startCol, int endCol);

        IMatrix<T> Add(IMatrix<T> right);

        IMatrix<T> Sub(IMatrix<T> right);


        IVector<T> Solve(IVector<T> fVector);

        T Det();
        IMatrix<T> Copy();
        IMatrix<T> Inv();
        string ToString();
        string ToString(string format);
    }
}