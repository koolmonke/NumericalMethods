using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MatrixArithmetic
{
    public class Matrix : IEnumerable<double>
    {
        public int N => _repr.GetLength(0);

        public int M => _repr.GetLength(1);

        public double this[int i, int j]
        {
            get => _repr[i, j];
            set => _repr[i, j] = value;
        }

        public Matrix From(IEnumerable<double> values)
        {
            using var enumerator = values.GetEnumerator();
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    if (!enumerator.MoveNext())
                    {
                        throw new MatrixDifferentDimException("Enumerable имеет другую размерность чем эта матрица");
                    }

                    this[i, j] = enumerator.Current;
                }
            }

            return this;
        }

        public Matrix Multiply(Matrix right)
        {
            var result = new Matrix(N, right.M);

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < right.M; j++)
                {
                    for (int k = 0; k < right.N; k++)
                    {
                        result[i, j] += this[i, k] * right[k, j];
                    }
                }
            }

            return result;
        }

        public static Matrix operator *(Matrix left, Matrix right) => left.Multiply(right);

        public Vector Multiply(Vector right)
        {
            var result = new Vector(N);

            for (int i = 0; i < N; i++)
            {
                for (int k = 0; k < M; k++)
                {
                    result[i] += this[i, k] * right[k];
                }
            }

            return result;
        }

        public static Vector operator *(Matrix left, Vector right) => left.Multiply(right);

        public Matrix Add(Matrix right)
        {
            if ((N, M) != (right.N, right.M))
            {
                throw new MatrixDifferentDimException();
            }

            return new Matrix(N, M).From(this.Zip(right).Select(item => item.First + item.Second));
        }

        public static Matrix operator +(Matrix left, Matrix right) => left.Add(right);

        public Matrix Sub(Matrix right)
        {
            if ((N, M) != (right.N, right.M))
            {
                throw new MatrixDifferentDimException();
            }

            return new Matrix(N, M).From(this.Zip(right).Select(item => item.First - item.Second));
        }

        public static Matrix operator -(Matrix left, Matrix right) => left.Sub(right);


        public Vector GetColumn(int index)
        {
            var vector = new Vector(M);

            for (int i = 0; i < M; i++)
            {
                vector[i] = this[index, i];
            }

            return vector;
        }

        public Matrix ExtractColumns(int[] cols)
        {
            cols = cols.Distinct().ToArray();
            Matrix output = new Matrix(N, cols.Length);

            for (int row = 0; row < N; row++)
            {
                int i = 0;
                for (int col = 0; col < M; col++)
                {
                    if (cols.Contains(col) == false)
                        continue;
                    output[row, i] = this[row, col];
                    i++;
                }
            }

            return output;
        }

        public void SwitchRows(int row1, int row2)
        {
            if (row1 == row2)
                return;

            for (int col = 0; col < M; col++)
            {
                (this[row1, col], this[row2, col]) = (this[row2, col], this[row1, col]);
            }
        }

        public Matrix ExtractColumns(int startCol, int endCol) =>
            ExtractColumns(Enumerable.Range(startCol, endCol - startCol + 1).ToArray());

        public Matrix ConcatHorizontally(Matrix other)
        {
            int m = M + other.M;
            Matrix output = new Matrix(N, m);
            for (int row = 0; row < N; row++)
            {
                for (int col = 0; col < M + other.M; col++)
                {
                    if (col < M)
                        output[row, col] = this[row, col];
                    else
                        output[row, col] = other[row, col - M];
                }
            }

            return output;
        }

        public Matrix Transpose()
        {
            var result = new Matrix(N, M);
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    result[i, j] = this[j, i];
                }
            }

            return result;
        }

        public static Matrix Identity(int n)
        {
            var matrix = new Matrix(n, n);

            for (int i = 0; i < n; i++)
            {
                matrix[i, i] = 1;
            }

            return matrix;
        }

        public Matrix Copy() => new Matrix(_repr);


        public IEnumerator<double> GetEnumerator()
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    yield return this[i, j];
                }
            }
        }

        public override string ToString() => ToString(MatrixElementFormat);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string ToString(string format)
        {
            var builder = new StringBuilder();


            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    builder.Append(
                        $"{this[i, j].ToString(format, CultureInfo.InvariantCulture)} ");
                }

                if (i < N - 1)
                {
                    builder.AppendLine();
                }
            }

            return builder.ToString();
        }

        private const string MatrixElementFormat = " #0.0000;-#0.0000; 0.0000";
        public string ToGridSolutionString()
        {
            var builder = new StringBuilder();
            
            for (int i = M - 1; i >= 0; i--)
            {
                builder.Append((i / 10d).ToString(MatrixElementFormat));
                for (int j = 0; j < N; j++)
                {
                    
                    builder.Append(this[j, i].ToString(MatrixElementFormat));
                }

                builder.AppendLine();
            }

            builder.Append(" T // X");
            for (int i = 0; i < N ; i++)
            {
                builder.Append((i / 10d).ToString(MatrixElementFormat));
            }

            builder.AppendLine();

            return builder.ToString();
        }

        public Matrix(double[,] values)
        {
            var rowCount = values.GetLength(0);
            var colCount = values.GetLength(1);

            var output = new double[rowCount, colCount];


            for (int row = 0; row < rowCount; row++)
            {
                for (int col = 0; col < colCount; col++)
                {
                    output[row, col] = values[row, col];
                }
            }

            _repr = output;
        }

        public Matrix(int n, int m)
        {
            _repr = new double[n, m];
        }

        private readonly double[,] _repr;
    }

    public class MatrixDifferentDimException : Exception
    {
        public MatrixDifferentDimException() : base("У этих матриц разная размерность")
        {
        }

        public MatrixDifferentDimException(string message) : base(message)
        {
        }
    }
}