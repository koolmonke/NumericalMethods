using System;
using System.Collections.Generic;
using MatrixArithmetic;

namespace C1.Lab2
{
    /// <summary>
    /// Решатель СЛАУ методом Гивенса
    /// </summary>
    public class GivensMethod
    {
        /// <param name="a">Матрица системы</param>
        /// <param name="f">Вектор свободных членов</param>
        public GivensMethod(Matrix a, Vector f)
        {
            System = a;
            FreeVector = f;
            _a = a;
            _f = f;
            _q = Matrix.Identity(a.N);
        }

        public Matrix System { get; }
        
        public Vector FreeVector { get; }

        /// <summary>
        /// Решение системы
        /// </summary>
        public Vector SolutionVector => _solutionVector ??= Solve();

        public Vector Residual => System.Multiply(SolutionVector).Sub(FreeVector);
        
        private Vector? _solutionVector;

        private Vector Solve()
        {
            for (int j = 0; j < _a.N - 1; j++)
            {
                for (int i = _a.N - 1; i > j; i--)
                {
                    if (System[i, j] == 0) continue;
                    _g.Add(GenerateG(i, j));
                    _a = _g[^1].Multiply(_a);
                }
            }

            if (_g.Count > 0)
            {
                ComputeQ();
            }

            return SolveUpper();
        }

        private Matrix _a, _q;
        
        private readonly Vector _f;
        
        private readonly List<Matrix> _g = new();

        
        private Vector SolveUpper()
        {
            _q = _q.Transpose();

            Vector qb = _q.Multiply(_f);
            var result = new Vector(qb.N);

            for (int i = qb.N - 1; i >= 0; i--)
            {
                double sum = 0;

                for (int j = i + 1; j < qb.N; j++)
                {
                    sum += result[j] * _a[i, j];
                }

                result[i] = (qb[i] - sum) / _a[i, i];
            }

            return result;
        }

        private void ComputeQ()
        {
            _g[0] = _g[0].Transpose();
            _q = _g[0];

            for (int i = 1; i < _g.Count; i++)
            {
                _g[i] = _g[i].Transpose();

                _q = _q.Multiply(_g[i]);
            }
        }

        private Matrix GenerateG(int i, int j)
        {
            double a = _a[i - 1, j];
            double b = _a[i, j];
            double r = Math.Sqrt(a * a + b * b);
            double c = a / r;
            double s = -b / r;

            Matrix g = Matrix.Identity(_a.N);


            g[i - 1, i - 1] = g[i, i] = c;
            g[i - 1, i] = -s;
            g[i, i - 1] = s;

            return g;
        }
    }
}