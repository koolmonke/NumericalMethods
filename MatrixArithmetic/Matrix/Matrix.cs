using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MatrixArithmetic.Solvers.Gauss;

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

        public Vector ToVectorByColumn(int column = 0)
        {
            if (M != 1)
            {
                throw new VectorDifferentDimException();
            }

            var vector = new Vector(N);

            for (int i = 0; i < N; i++)
            {
                vector[i] = this[i, column];
            }

            return vector;
        }

        public Vector ToVectorByRow(int row = 0)
        {
            if (M != 1)
            {
                throw new VectorDifferentDimException();
            }

            var vector = new Vector(M);

            for (int i = 0; i < M; i++)
            {
                vector[i] = this[row, i];
            }

            return vector;
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


        public Vector Solve(Vector fVector) => new GaussSolver(this, fVector).SolutionVector;

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

        public double Det()
        {
            Matrix matrix = Copy();
            var n = matrix.N;
            double det = 1;
            for (int i = 0; i < n; i++)
            {
                int k = i;
                for (int j = i + 1; j < n; j++)
                {
                    if (Math.Abs(matrix[j, i]) > Math.Abs(matrix[k, i]))
                    {
                        k = j;
                    }
                }

                if (Math.Abs(matrix[k, i]) < Constants.Epsilon)
                {
                    return 0;
                }

                if (i != k)
                {
                    det = -det;
                }

                matrix.SwitchRows(i, k);

                det *= matrix[i, i];
                for (int j = i + 1; j < n; j++)
                {
                    matrix[i, j] /= matrix[i, i];
                }

                for (int j = 0; j < n; j++)
                {
                    if (j != i && Math.Abs(matrix[j, i]) > Constants.Epsilon)
                    {
                        for (k = i + 1; k < n; k++)
                        {
                            matrix[j, k] -= matrix[i, k] * matrix[j, i];
                        }
                    }
                }
            }

            return det;
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

        public Matrix Inv()
        {
            var vectors = ParallelEnumerable.Range(0, N).AsOrdered().Select(i =>
            {
                var tmpVector = new Vector(N)
                {
                    [i] = 1
                };

                return Solve(tmpVector);
            }).ToArray();

            var firstVectorN = vectors[0].N;

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

        public override string ToString() => ToString(" #0.0000;-#0.0000; 0.0000");

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

        public Matrix(double[,] values)
        {
            _repr = values.CreateCopy();
        }

        public Matrix(int n, int m)
        {
            _repr = new double[n, m];
        }

        private readonly double[,] _repr;
    }
}