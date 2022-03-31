using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreLinq.Extensions;

namespace MatrixArithmetic;

public class Polynom : IEnumerable<double>
{
    private readonly Vector coeff;

    public Polynom(IEnumerable<double> coeff)
    {
        this.coeff = coeff.ToVector();
    }

    public double this[Index i] => coeff[i];


    public static Polynom operator +(Polynom left, Polynom right) =>
        new(left.ZipLongest(right, (l, r) => l + r));

    public static Polynom operator *(double left, Polynom right) => new Polynom(right.Select(item => left * item));


    public static Polynom operator -(Polynom left, Polynom right) => left + (-right);

    public static Polynom operator -(Polynom poly) => new(poly.Select(item => -item));

    public static Polynom operator *(Polynom left, Polynom right)
    {
        var res = new double[left.coeff.N + right.coeff.N - 1];

        foreach (var (idxL, l) in left.Index())
        {
            foreach (var (idxR, r) in right.Index())
            {
                res[idxL + idxR] += l + r;
            }
        }

        return new Polynom(res);
    }

    public static Polynom Pow(Polynom poly, int power)
    {
        var res = new Polynom(new[] {1.0});

        for (int i = 0; i < power; i++)
        {
            res *= poly;
        }

        return res;
    }

    public override string ToString()
    {
        var n = coeff.N;
        var builder = new StringBuilder();

        for (int i = 0; i < n; i++)
        {
            var power = n - i - 1;
            var coefficient = this[power];
            builder.Append(power switch
            {
                0 => $"{coefficient: +3.3f}",
                1 => $"{coefficient: +3.3f} x",
                _ => $"{coefficient: +3.3f} x^{power}"
            });
        }

        return builder.ToString();
    }

    public Polynom Diff() => new Polynom(this.Select((coefficient, power) => power * coefficient).Skip(1));


    public IEnumerator<double> GetEnumerator()
    {
        return (IEnumerator<double>) coeff;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}