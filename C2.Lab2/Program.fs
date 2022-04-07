open System
open MatrixArithmetic

let M = 10

let N = 10

let var = 19

let a = 1

let l = 1

let T = 1

let h = float l / float N

let Mu = float T / float N

let ALPHA = 0.5 + 0.1 * float var

let s = (Mu ** 2 / h ** 2 * float a)

let phi0 t = 1. / (ALPHA * t + 1.)

let phi1 t = 1. / (ALPHA * t + 2.)

let alpha x = 1. / (x + 1.)

let d2Alpha x = 2. / (x + 1.) ** 3.

let beta x = -ALPHA / (1. + x) ** 2.

let func x t =
    2. * (ALPHA ** 2. - 1.)
    / ((x + ALPHA * t + 1.) ** 3.)

let Yi0 = alpha

let Yi1 x =
    alpha x
    + Mu * beta x
    + Mu ** 2. / 2.
      * (float a ** 2. * d2Alpha x + func x 0.)

let calcY (x: Vector) (t: Vector) =
    let m = Matrix(N + 1, M + 1)
    for j in 0..M do
        m[0, j] <- phi0 t[j]
        m[M, j] <- phi1 t[j]
    for i in 1..(N-1) do
        m[i, 0] <- Yi0 t[i]
        m[i, 1] <- Yi1 t[i]
    for j in 1..(M-1) do
        for i in 1..(N-1) do
            m[i, j+1] <- s * m[i + 1, j] + 2. * (1. - s) * m[i,j] + s * m[i - 1, j] - m[i, j - 1] + Mu ** 2 * func x[i] t[j]
    m

let x =
    Vector(seq { for i in 0 .. N -> float i * h })

let t =
    Vector(seq { for i in 0 .. N -> float i * Mu })
    
let matrixY = calcY x t

Console.WriteLine(matrixY.ToGridSolutionString())
