namespace MatrixArithmetic.Solvers
{
    public interface ISolver
    {
        public Matrix System { get; }
        
        public Vector FreeVector { get; }
        
        public Vector SolutionVector { get; }

        Vector Solve();

        Vector Residual();
    }
}