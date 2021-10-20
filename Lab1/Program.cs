using System;
using System.Globalization;
using MatrixArithmetic;
using MatrixArithmetic.Solvers;

namespace Lab1
{
    internal static class Program
    {
        private static void Main()
        {
            Console.WriteLine("Матрица A");
            var a = Matrix.From(new[,]
            {
                { -2, 3.01, .12, -.11 },
                { 2.92, -.17, .11, .22 },
                { .66, .52, 3.17, 2.11 },
                { 3.01, .42, -.27, .15 }
            });
            Console.WriteLine(a);

            Console.WriteLine("Вектор f");
            var f = Vector.From(new[]
            {
                4.13,
                3.46,
                2.79,
                1.01
            });
            Console.WriteLine(f);

            Console.WriteLine("её детерминант");
            var det = a.Det();
            Console.WriteLine(det.ToString(CultureInfo.InvariantCulture));

            Console.WriteLine("решение ax=f");
            var solver = new GaussSolver(a, f);
            Console.WriteLine(solver.SolutionVector);

            Console.WriteLine("Вектор невязки");
            var residual = solver.Residual();
            Console.WriteLine(residual
                .ToString(" #0.000000000000;-#0.000000000000;0.000000000000"));

            Console.WriteLine("обратная матрица a");
            var matrixInv = a.Inv();
            Console.WriteLine(matrixInv);

            Console.WriteLine("матрица а умноженный на её обратную матрицу");
            var multiplyResult = matrixInv.Multiply(a);
            Console.WriteLine(multiplyResult);
        }
    }
}