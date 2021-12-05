using System;
using MatrixArithmetic;
using MatrixArithmetic.Norms;
using static System.Math;

namespace Lab3
{
    internal static class Program
    {
        private static double Sec(double x) => 1 / Cos(x);

        private static void Main()
        {
            // Изначальный вектор-функция
            Func<Vector, double>[] originalVector =
            {
                vector => vector[0] + Cos(vector[1]) - vector[0] * vector[0] * Sin(vector[2] * vector[2]) - 0.2,
                vector => Tan(vector[0]) - vector[1] * Sin(1 - vector[2]) - vector[1] + 0.1,
                vector => Sin(vector[0] + vector[1]) + 2 * vector[1] + 2 * vector[2] - 0.1,
            }; 

            // Якобиан для вектор-функции
            Func<Vector, double>[,] jacobian =
            {
                {
                    vector => 1 - 2 * vector[0] * Sin(vector[2] * vector[2]),
                    vector => -Sin(vector[1]),
                    vector => -2 * vector[0] * vector[0] * vector[2] * Cos(vector[2] * vector[2])
                },
                {
                    vector => Pow(Sec(vector[0]), 2),
                    vector => -Sin(1 - vector[2]) - 1,
                    vector => vector[1] * Cos(1 - vector[2])
                },
                {
                    vector => Cos(vector[0] + vector[1]),
                    vector => Cos(vector[0] + vector[1]) + 2,
                    _ => 2
                }
            };

            var guess = new Vector(new double[] {4, 1, 0});

            var taxiCabNorm = new TaxiCabNorm();
            var gradient = new Gradient(jacobian, originalVector, taxiCabNorm, guess);

            Console.WriteLine("Вектор решений методом градиента");
            Console.WriteLine(gradient.SolutionVector);
            Console.WriteLine("Вектор невязки для метода градиента");
            Console.WriteLine(originalVector.Apply(gradient.SolutionVector).ToResidualString());
            Console.WriteLine($"Кол-во итераций для метода градиента {gradient.CounterIteration}");

            var newton = new Newton(jacobian, originalVector, taxiCabNorm, gradient.SolutionVector);

            Console.WriteLine("Вектор решений методом Ньютона");
            Console.WriteLine(newton.SolutionVector.ToString(" #0.0000000;-#0.0000000;0.0000000"));
            Console.WriteLine("Вектор невязки для метода Ньютона");
            Console.WriteLine(originalVector.Apply(newton.SolutionVector).ToResidualString());
            Console.WriteLine($"Кол-во итераций для метода Ньютона {newton.CounterIteration}");
        }
    }
}