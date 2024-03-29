﻿using MatrixArithmetic;

const double variant = 19;
const int limitT = 6;
const int limitX = 11;
const double stepX = 0.1;
const double stepT = 0.01;
const double s = stepT / (stepX * stepX);
// Coefficients for TMA
const double c = 1.0 + 2.0 * s;

var nodesX = Enumerable.Range(0, limitX).Select(item => item * stepX).ToVector();
var nodesT = Enumerable.Range(0, limitT).Select(item => item * stepT).ToVector();

var explicitFiniteDifferenceScheme = new Matrix(limitX, limitT);

double EdgeFunc(double t) => variant / 2 * t;

double FunctionF(double x, double t) => variant / 2 * (Math.Pow(x, 2) - 2 * t);

// Explicit Finite Difference Scheme
for (int t = 0; t < limitT; ++t)
{
    explicitFiniteDifferenceScheme[limitX - 1, t] = EdgeFunc(nodesT[t]);
}

for (int col = 0; col < limitT - 1; ++col)
{
    for (int row = 1; row < limitX - 1; ++row)
    {
        explicitFiniteDifferenceScheme[row, col + 1] =
            s * explicitFiniteDifferenceScheme[row - 1, col] +
            (1 - 2 * s) * explicitFiniteDifferenceScheme[row, col] +
            s * explicitFiniteDifferenceScheme[row + 1, col] +
            stepT * FunctionF(nodesX[row], nodesT[col]);
    }
}

Console.WriteLine(explicitFiniteDifferenceScheme.ToGridSolutionString());

var implicitFiniteDifferenceScheme = new Matrix(limitX, limitT);
// Implicit Finite Difference Scheme 
for (int t = 1; t < limitT; ++t)
{
    // Find other TMA coefficients alpha and betta
    // Source alpha_0 and betta_0, which are boundary values, where alpha_0 = betta_0 = 0
    Vector alpha = new(limitX);
    Vector betta = new(limitX)
    {
        [limitX - 1] = EdgeFunc(nodesT[t])
    };
    for (int x = 1; x < limitX; ++x)
    {
        // Find other alpha_n and betta_n, n > 1
        alpha[x] = s / (c - alpha[x - 1] * s);
        betta[x] = (stepT * FunctionF(nodesX[x], nodesT[t]) +
                    implicitFiniteDifferenceScheme[x, t - 1] + s * betta[x - 1]) /
                   (c - alpha[x - 1] * s);
    }

    implicitFiniteDifferenceScheme[limitX - 1, t] = EdgeFunc(nodesT[t]);
    for (int x = 9; x > 0; --x)
    {
        implicitFiniteDifferenceScheme[x, t] = alpha[x] * implicitFiniteDifferenceScheme[x + 1, t] + betta[x];
    }
}

Console.WriteLine(implicitFiniteDifferenceScheme.ToGridSolutionString());