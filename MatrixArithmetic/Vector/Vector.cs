using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static System.Math;

namespace MatrixArithmetic
{
    public class Vector : IVector<double>
    {
        IEnumerator<double> IEnumerable<double>.GetEnumerator() => ((IEnumerable<double>)Repr).GetEnumerator();

        public IEnumerator GetEnumerator() => Repr.GetEnumerator();

        public int N => Repr.Length;

        public double this[Index i]
        {
            get => Repr[i];
            set => Repr[i] = value;
        }

        public IVector<double> this[Range i] => new Vector(Repr[i]);

        public double Norm() => Sqrt(this * this);

        public static Vector WithSize(int n) => new Vector(n);

        public static Vector From(double[] values) => new Vector(values);

        public IVector<double> Copy() => From(Repr);
        public double[] ToRepresentation()
        {
            return new Vector(this.Repr).Repr;
        }

        public static double operator *(Vector self, Vector other)
        {
            if (self.N != other.N)
            {
                throw new VectorDifferentDimException();
            }

            return self.Zip(other).Select(pair => pair.First * pair.Second).Sum();
        }

        public static Vector operator *(Vector self, double other) => self.Select(value => other * value).ToVector();

        public static Vector operator *(double self, Vector other) => other * self;

        public static Vector operator +(Vector self, Vector other)
        {
            if (self.N != other.N)
            {
                throw new VectorDifferentDimException();
            }

            return self.Zip(other).Select(pair => pair.First + pair.Second).ToVector();
        }

        public static Vector operator -(Vector self, Vector other)
        {
            if (self.N != other.N)
            {
                throw new VectorDifferentDimException();
            }

            return self.Zip(other).Select(pair => pair.First - pair.Second).ToVector();
        }

        public static Vector operator -(Vector self) => self.Select(value => -value).ToVector();

        public override string ToString()
        {
            return ToString(" #0.0000;-#0.000;0.0000");
        }

        public string ToString(string format) => string.Join('\n',
            this.Select(value => value.ToString(format, CultureInfo.InvariantCulture)));

        public IVector<double> Sub(IVector<double> vector)
        {
            return this - (vector as Vector)!;
        }


        private double[] Repr;

        private Vector(double[] values)
        {
            Repr = values.Select(value => value).ToArray();
        }

        private Vector(int n)
        {
            Repr = new double[n];
        }
    }
}