namespace MatrixArithmetic.Norms
{
    public interface INorma
    {
        double VectorNorm(Vector vector);
        double MatrixNorm(Matrix matrix);
    }
}