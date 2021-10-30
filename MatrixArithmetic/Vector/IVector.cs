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
        IVector<T> Sub(IVector<T> vector);
        IVector<T> Sub(T item);
        IVector<T> Add(IVector<T> vector);
        IVector<T> Multiply(T value);
        T Multiply(IVector<T> value);

        IMatrix<T> ToMatrix();
    }
}