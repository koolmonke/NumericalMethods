namespace MatrixArithmetic.Norms
{
    public interface INorma
    {
        public double VectorNorm(IVector<double> vector);
        public double MatrixNorm(IMatrix<double> matrix);
    }
}