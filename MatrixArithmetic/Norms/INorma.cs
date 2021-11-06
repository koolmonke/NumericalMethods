namespace MatrixArithmetic.Norms
{
    public interface INorma
    {
        public double VectorNorm(Vector vector);
        public double MatrixNorm(Matrix matrix);
    }
}