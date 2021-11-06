using System;

namespace MatrixArithmetic.Solvers.Gauss
{
    public class GaussSolver : ISolver
    {
        public GaussSolver(Matrix matrix, Vector forVector)
        {
            this.System = matrix;
            this.FreeVector = forVector;
        }

        public Vector FreeVector { get; }

        public Matrix System { get; }

        private Vector? _solutionVector;

        public Vector SolutionVector => _solutionVector ??= this.Solve();

        public Vector Solve()
        {
            var newMatrix = System.ConcatHorizontally(FreeVector.ToMatrix());

            var fullMatrix = Eliminate(newMatrix, MatrixReductionForm.ReducedRowEchelonForm, 1)
                .FullMatrix;

            Vector result = new Vector(System.N);

            for (int i = 0; i < System.N; i++)
            {
                result[i] = fullMatrix[i, System.N];
            }

            return result;
        }


        public Vector Residual() => System.Multiply(SolutionVector.ToMatrix()).ToVectorByColumn().Sub(FreeVector);


        /// <summary>
        /// Reduces matrix to row-echelon (REF/Gauss) or reduced row-echelon (RREF/Gauss-Jordan) form and solves for augmented columns (if any.)
        /// </summary>
        private static MatrixEliminationResult Eliminate(Matrix input, MatrixReductionForm form,
            int augmentedCols = 0)
        {
            var result = new MatrixEliminationResult();

            var output = input.Copy();

            int numPivots = 0;

            for (int col = 0; col < input.M - augmentedCols; col++)
            {
                int? pivotRow = FindPivot(output, numPivots, col, input.N);

                if (pivotRow == null)
                    continue;

                ReduceRow(output, pivotRow.Value, col, input.M);

                output.SwitchRows(pivotRow.Value, numPivots);


                pivotRow = numPivots;
                numPivots++;

                if (form == MatrixReductionForm.ReducedRowEchelonForm)
                {
                    for (int tmpRow = 0; tmpRow < pivotRow; tmpRow++)
                        EliminateRow(output, tmpRow, pivotRow.Value, col, input.M);
                }

                for (int tmpRow = pivotRow.Value; tmpRow < input.N; tmpRow++)
                    EliminateRow(output, tmpRow, pivotRow.Value, col, input.M);
            }

            result.FullMatrix = output;
            result.UnknownsCount = augmentedCols;
            result.TotalRowCount = input.N;
            result.TotalColumnCount = input.M;
            result.AugmentedColumnCount = augmentedCols;

            result.AugmentedColumns = output.ExtractColumns(result.UnknownsCount, input.M - 1);
            if (augmentedCols > 0 && form == MatrixReductionForm.ReducedRowEchelonForm)
            {
                // matrix has solution 
                result = FindSolution(result);
            }

            return result;
        }

        private static MatrixEliminationResult FindSolution(MatrixEliminationResult result)
        {
            double?[]? solution = null;


            if (result.AugmentedColumnCount == 1)
            {
                var solutionState = result.UnknownsCount > result.TotalRowCount
                    ? MatrixSolutionState.Infinite
                    : MatrixSolutionState.Unique;

                solution = new double?[result.UnknownsCount];

                int pivotRow = 0;
                for (int col = 0; col < result.UnknownsCount; col++)
                {
                    double unknownValue = result.FullMatrix[pivotRow, col];
                    double solutionValue = result.FullMatrix[pivotRow, result.TotalColumnCount - 1];

                    if (Math.Abs(unknownValue - 1) < Constants.Epsilon)
                    {
                        solution[col] = solutionValue;

                        pivotRow++;
                    }
                    else
                    {
                        if (solutionState == MatrixSolutionState.Unique)
                        {
                            // still on default value
                            solutionState = solutionValue == 0
                                ? MatrixSolutionState.Infinite
                                : MatrixSolutionState.None;
                        }
                    }
                }
            }

            result.Solution = solution;

            return result;
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

            if (Math.Abs(coefficient - 1) < Constants.Epsilon)
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