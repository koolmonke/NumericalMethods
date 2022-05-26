using System;
using System.Linq;
using MatrixArithmetic;

namespace C1.Lab1
{
    /// <summary>
    /// Решатель СЛАУ методом Гаусса
    /// </summary>
    public class GaussSolver
    {
        /// <param name="matrix">Матрица системы</param>
        /// <param name="forVector">Вектор свободных членов</param>
        public GaussSolver(Matrix matrix, Vector forVector)
        {
            System = matrix;
            FreeVector = forVector;
        }

        /// <summary>
        /// Нахождение детерминанта методом Гаусса
        /// </summary>
        /// <param name="system">Матрица для которой нужно найти детерминант</param>
        /// <returns>Детерминант</returns>
        public static double Det(Matrix system)
        {
            Matrix matrix = system.Copy();
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

                if (Math.Abs(matrix[k, i]) < 1e-10)
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
                    if (j != i && Math.Abs(matrix[j, i]) > 1e-10)
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
        /// <summary>
        /// Нахождение обратной матрицы методом Гаусса через решение N СЛАУ
        /// </summary>
        /// <param name="system">матрица размерности N*N обратную матрицу которой нужно найти</param>
        /// <returns>обратная матрица</returns>
        public static Matrix Inv(Matrix system)
        {
            var vectors = ParallelEnumerable.Range(0, system.N)
                .AsOrdered()
                .Select(i => new Vector(system.N) {[i] = 1})
                .Select(vector => new GaussSolver(system, vector).SolutionVector);


            var result = new Matrix(system.N, system.N);

            foreach (var (vector, i) in vectors.Select((vector, i) => (vector, i)))
            {
                for (int j = 0; j < system.N; j++)
                {
                    result[j, i] = vector[j];
                }
            }

            return result;
        }

        public Vector FreeVector { get; }

        public Matrix System { get; }

        /// <summary>
        /// Вектор решения системы
        /// </summary>
        public Vector SolutionVector => _solutionVector ??= Solve();

        private Vector? _solutionVector;

        private Vector Solve()
        {
            var newMatrix = System.ConcatHorizontally(FreeVector.ToMatrix());

            var fullMatrix = Eliminate(newMatrix);

            var result = new Vector(System.N);

            for (int i = 0; i < System.N; i++)
            {
                result[i] = fullMatrix[i, System.N];
            }

            return result;
        }


        /// <summary>
        /// Проверка решения
        /// </summary>
        public Vector Residual => System * SolutionVector - FreeVector;

        private static Matrix Eliminate(Matrix input)
        {
            var output = input.Copy();

            int numPivots = 0;

            for (int col = 0; col < input.M - 1; col++)
            {
                int? pivotRow = FindPivot(output, numPivots, col, input.N);

                if (pivotRow == null)
                    continue;

                ReduceRow(output, pivotRow.Value, col, input.M);

                output.SwitchRows(pivotRow.Value, numPivots);


                pivotRow = numPivots;
                numPivots++;

                for (int tmpRow = 0; tmpRow < pivotRow; tmpRow++)
                    EliminateRow(output, tmpRow, pivotRow.Value, col, input.M);

                for (int tmpRow = pivotRow.Value; tmpRow < input.N; tmpRow++)
                    EliminateRow(output, tmpRow, pivotRow.Value, col, input.M);
            }


            return output;
        }

        private static int? FindPivot(Matrix input, int startRow, int col, int rowCount)
        {
            for (int i = startRow; i < rowCount; i++)
            {
                if (input[i, col] != 0)
                    return i;
            }

            return null;
        }

        private static void ReduceRow(Matrix input, int row, int col, int colCount)
        {
            var coefficient = 1.0 / input[row, col];

            if (Math.Abs(coefficient - 1) < 1e-10)
                return;

            for (; col < colCount; col++)
                input[row, col] *= coefficient;
        }

        private static void EliminateRow(Matrix input, int row, int pivotRow, int pivotCol, int colCount)
        {
            if (pivotRow == row)
                return;

            if (input[row, pivotCol] == 0)
                return;

            double coefficient = input[row, pivotCol];

            for (int col = pivotCol; col < colCount; col++)
            {
                input[row, col] -= input[pivotRow, col] * coefficient;
            }
        }
    }
}