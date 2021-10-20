using System;

namespace MatrixArithmetic
{
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