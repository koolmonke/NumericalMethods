using System;
using MatrixArithmetic;
using MatrixArithmetic.Norms;
using MatrixArithmetic.Solvers;

namespace Lab2
{
    class Program
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

            var rotationSolver = new RotationSolver(a, f);

            Console.WriteLine("Решение методом вращений");
            Console.WriteLine(rotationSolver.SolutionVector);
            Console.WriteLine(rotationSolver.Residual());

            var simpleIterationSolver = new SimpleIterationSolver(new TaxiCabNorm(), a, f);
            
            Console.WriteLine("Решение методом простых итераций");
            Console.WriteLine(rotationSolver.SolutionVector);
            Console.WriteLine(simpleIterationSolver.Residual());

        }
    }
}