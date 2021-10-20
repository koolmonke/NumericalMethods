namespace MatrixArithmetic.Solvers
{
    public interface ISolver<T>
    {
        public IMatrix<T> Matrix { get; }
        public IVector<T> Vector { get; }
        
        
        
        public IVector<T> SolutionVector { get; }

        IVector<T> Solve();

        IVector<T> Residual();
    }
}