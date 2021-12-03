using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MatrixArithmetic
{
    public class Vector : IEnumerable<double>
    {
        IEnumerator<double> IEnumerable<double>.GetEnumerator() => ((IEnumerable<double>) _repr).GetEnumerator();

        public IEnumerator GetEnumerator() => _repr.GetEnumerator();

        public int N => _repr.Length;

        public double this[Index i]
        {
            get => _repr[i];
            set => _repr[i] = value;
        }

        public Vector this[Range i] => new Vector(_repr[i]);

        public Vector Copy() => new Vector(_repr);

        public static double operator *(Vector self, Vector other)
        {
            if (self.N != other.N)
            {
                throw new VectorDifferentDimException();
            }

            return self.Zip(other).Select(pair => pair.First * pair.Second).Sum();
        }

        public static Vector operator *(Vector self, double other) => self.Select(value => other * value).ToVector();
        public static Vector operator *(double other, Vector self) => self * other;

        public override string ToString() => ToString(" #0.0000;-#0.000;0.0000");

        public string ToResidualString() => ToString(" #0.000000000000;-#0.000000000000; 0.000000000000");

        public string ToString(string format) => string.Join(Environment.NewLine,
            this.Select(value => value.ToString(format, CultureInfo.InvariantCulture)));

        public Vector Sub(Vector vector)
        {
            if (N != vector.N)
            {
                throw new VectorDifferentDimException();
            }

            return this.Zip(vector).Select(pair => pair.First - pair.Second).ToVector();
        }

        public static Vector operator -(Vector left, Vector right) => left.Sub(right);
        public static Vector operator -(Vector vector) => vector.Select(item => -item).ToVector();

        public Vector Add(Vector vector)
        {
            if (N != vector.N)
            {
                throw new VectorDifferentDimException();
            }

            return this.Zip(vector).Select(pair => pair.First + pair.Second).ToVector();
        }

        public static Vector operator +(Vector left, Vector right) => left.Add(right);


        public Vector Multiply(double value)
        {
            return this.Select(item => item * value).ToVector();
        }


        public Matrix ToMatrix()
        {
            var n = N;
            var result = new Matrix(n, 1);

            for (int i = 0; i < n; i++)
            {
                result[i, 0] = this[i];
            }

            return result;
        }

        public Vector Sub(double item)
        {
            return this.Select(value => value - item).ToVector();
        }


        public Vector(IEnumerable<double> values)
        {
            _repr = values.Select(value => value).ToArray();
        }

        public Vector(int n)
        {
            _repr = new double[n];
        }

        private readonly double[] _repr;
    }
}