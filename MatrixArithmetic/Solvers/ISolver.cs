namespace MatrixArithmetic.Solvers
{
    public interface ISolver<T>
    {
        public IMatrix<T> System { get; }
        
        public IVector<T> FreeVector { get; }
        
        public IVector<T> SolutionVector { get; }

        IVector<T> Solve();

        IVector<T> Residual();
    }
}