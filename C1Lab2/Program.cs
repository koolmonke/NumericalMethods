using System;
using C1Lab1;
using MatrixArithmetic;
using MatrixArithmetic.Norms;

namespace C1Lab2
{
    internal static class Program
    {
        private static void Main()
        {
            var a = new Matrix(new double[,]
            {
                { 19, -4, 6, -1 },
                { -4, 20, -2, 7 },
                { 6, -2, 25, -4 },
                { -1, 7, -4, 15 }
            });

            var f = new Vector(new double[]
            {
                100,
                -5,
                34,
                69
            });

            Console.WriteLine("Матрица A:");
            Console.WriteLine(a);

            var taxiCabNorm = new TaxiCabNorm();
            var matrixANorm = taxiCabNorm.MatrixNorm(a);
            var matrixAInvNorm = taxiCabNorm.MatrixNorm(GaussSolver.Inv(a));

            Console.WriteLine($"Число обусловленности {matrixANorm * matrixAInvNorm}");

            Console.WriteLine("Вектор f:");
            Console.WriteLine(f);

            var rotationSolver = new GivensMethod(a, f);

            Console.WriteLine("Решение методом вращений");
            Console.WriteLine(rotationSolver.SolutionVector);
            Console.WriteLine("Вектор невязки для метода вращений");
            Console.WriteLine(rotationSolver.Residual.ToResidualString());

            var simpleIterationSolver = new SimpleIterationSolver(taxiCabNorm, a, f);

            Console.WriteLine("Решение методом простых итераций");
            Console.WriteLine(simpleIterationSolver.SolutionVector);
            Console.WriteLine("Вектор невязки для метода простых итераций");
            Console.WriteLine(simpleIterationSolver.Residual.ToResidualString());
        }
    }
}