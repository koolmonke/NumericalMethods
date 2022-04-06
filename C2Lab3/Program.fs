open System
open MatrixArithmetic

let variant = 19

let alpha = 0.5 * float variant

let func x t = alpha * (x ** 2. - 2. * t)

let phi0 t = 0

let phi1 t = alpha * t

let psi t = 0


let T = 0.05

let l = 1.

let h = 0.1

let tau = 0.01

let S = tau / h ** 2.
let N = int(l / h) + 1
let M = int(T / tau) + 1

let xI = Vector(seq {for i in 0..(N-1) -> float i * h})
let tJ = Vector(seq {for j in 0..(M-1) -> float j * tau})

let Uij = Matrix(N, M)

for j in 0..(M-1) do
    Uij[0, j] <- phi0 tJ.[j]
    Uij[N-1, j] <- phi1 tJ.[j]
    
for i in 0..(M-1) do
    Uij[i, 0] <- psi tJ[i]

let a = S
let b = S
let c = 1. - 2. * S

for j in 0..(M-2) do
    for i in 1..(N-2) do
        Uij[i, j+1] <- a * Uij[i - 1, j] + c * Uij[i, j] + b * Uij[i + 1, j] + tau * func xI[i] tJ[j]

Console.WriteLine(Uij)