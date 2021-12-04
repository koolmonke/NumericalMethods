using System;
using System.Globalization;
using MatrixArithmetic;
using MatrixArithmetic.Gauss;

namespace Lab1
{
    internal static class Program
    {
        private static void Main()
        {
            Console.WriteLine("Матрица A");
            var a = new Matrix(new[,]
            {
                { -2, 3.01, .12, -.11 },
                { 2.92, -.17, .11, .22 },
                { .66, .52, 3.17, 2.11 },
                { 3.01, .42, -.27, .15 }
            });
            Console.WriteLine(a);

            Console.WriteLine("Вектор f");
            var f = new Vector(new[]
            {
                4.13,
                3.46,
                2.79,
                1.01
            });
            Console.WriteLine(f);

            Console.WriteLine("её детерминант");
            Console.WriteLine(a.Det().ToString(CultureInfo.InvariantCulture));

            Console.WriteLine("решение ax=f");
            var solver = new GaussSolver(a, f);
            Console.WriteLine(solver.SolutionVector);

            Console.WriteLine("Вектор невязки");
            Console.WriteLine(solver.Residual.ToResidualString());

            Console.WriteLine("обратная матрица a");
            var matrixInv = a.Inv();
            Console.WriteLine(matrixInv);

            Console.WriteLine("Обратная матрица а умноженная на матрицу а");
            var multiplyResult = matrixInv.Multiply(a);
            Console.WriteLine(multiplyResult);
        }
    }
}