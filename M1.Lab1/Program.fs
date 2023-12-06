module M1.Lab1

open System

let q x a b = 0.25 * x / ((b - a) ** 2.0)

let f x a b mu1 mu2 =
    (0.25 / ((b - a) ** 2.0))
    * ((Math.PI ** 2.0 + x) * (mu2 - mu1) * sin (Math.PI * (x - a) / 2.0 / (b - a)) + x)

let exactFunc x a b mu1 mu2 =
    mu1 + (mu2 - mu1) * sin (Math.PI * (x - a) / 2.0 / (b - a))

let k x = (1. + x) ** 2.

let solveDifferentialEquation a b mu1 mu2 N =
    let x = Array.init (N + 1) (fun i -> a + float i * ((b - a) / float N))
    let dx = (b - a) / float N

    let U = Array.zeroCreate (N + 1)
    let i0 = N / 2

    let alpha = Array.zeroCreate (N + 1)
    let betta = Array.zeroCreate (N + 1)
    let ksi = Array.zeroCreate (N + 1)
    let eta = Array.zeroCreate (N + 1)

    alpha.[1] <- 0.0
    betta.[1] <- mu1
    ksi.[N] <- 0.0
    eta.[N] <- mu2

    for i in 1..i0 do
        let A = 1.0
        let B = 1.0
        let C = 2.0 + dx * dx * q x.[i] a b
        let F = dx * dx * f x.[i] a b mu1 mu2

        alpha.[i + 1] <- B / (C - A * alpha.[i])
        betta.[i + 1] <- (F + A * betta.[i]) / (C - A * alpha.[i])

    for i in (N - 1) .. -1 .. i0 do
        let A = 1.0
        let B = 1.0
        let C = 2.0 + dx * dx * q x.[i] a b
        let F = dx * dx * f x.[i] a b mu1 mu2

        ksi.[i] <- A / (C - B * ksi.[i + 1])
        eta.[i] <- (F + B * eta.[i + 1]) / (C - B * ksi.[i + 1])

    U.[i0] <-
        (betta.[i0 + 1] + alpha.[i0 + 1] * eta.[i0 + 1])
        / (1.0 - alpha.[i0 + 1] * ksi.[i0 + 1])

    for i in (i0 - 1) .. -1 .. 0 do
        U.[i] <- alpha.[i + 1] * U.[i + 1] + betta.[i + 1]

    for i in i0 .. N - 1 do
        U.[i + 1] <- ksi.[i + 1] * U.[i] + eta.[i + 1]

    x, U


let a = 0.0
let b = 1.0
let mu1 = 1.0 (*Условие коши в a*)
let mu2 = 2.0 (*Условие коши в b*)
let N = 10


let x, U = solveDifferentialEquation a b mu1 mu2 N

printfn "Хакимов Артур вариант 29 встречная прогонка"
printfn "%-10s %-10s %-10s %-10s" "x" "y" "u" "r"

for i in 0..N do
    printfn "%.4f %10.4f %10.4f %14.8f" x.[i] U.[i] (exactFunc x.[i] a b mu1 mu2) (U.[i] - exactFunc x.[i] a b mu1 mu2)
