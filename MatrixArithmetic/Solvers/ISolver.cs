namespace MatrixArithmetic.Solvers
{
    public interface ISolver
    {
        Matrix System { get; }

        Vector FreeVector { get; }

        Vector SolutionVector { get; }

        Vector Residual { get; }
    }
}