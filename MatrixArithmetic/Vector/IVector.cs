using System;
using System.Collections.Generic;

namespace MatrixArithmetic
{
    public interface IVector<T> : IEnumerable<T>
    {
        int N { get; }

        T this[Index i] { get; set; }

        IVector<T> this[Range i] { get; }

        T Norm();
        IVector<T> Copy();

        T[] ToRepresentation();
        string ToString();
        string ToString(string format);
        IVector<double> Sub(IVector<double> doubles);
    }
}