module M1.Lab2

let pi = System.Math.PI

let a = 0.0
let b = 1.0
let n = 10
let h = (b - a) / float n
let beta1 = 0.5
let aVal = 1.0
let bVal = 1.0

let u x =
    1.0 + sin (pi * (x - a) / (2.0 * (b - a)))

let du x =
    (pi * cos (pi * (x / (2.0 * (b - a)) - a / (2.0 * (b - a))))) / (2.0 * (b - a))

let k x =
    - sin((pi * (x - a)) / (2.0 * (b - a))) - 0.315

let q x = x / ((b - a) ** 2.0) / 4.0

let bigF x =
    (0.25 / ((b - a) ** 2.0))
    * ((pi ** 2.0 + x) * sin (pi * (x - a) / (2.0 * (b - a))) + x)

let mu1 = beta1 * u a - k a * du a
let mu2 = u b
let y = Array.zeroCreate (n + 1)
let uValues = Array.init (n + 1) (fun i -> u (a + float i * h))
let xValues = Array.init (n + 2) (fun i -> a + float i * h)
let alpha = Array.zeroCreate (n + 1)
let beta = Array.zeroCreate (n + 1)
let ksi = Array.zeroCreate (n + 1)
let eta = Array.zeroCreate (n + 1)

beta.[1] <- mu1
eta.[n] <- mu2

for i in 1 .. n / 2 do
    let cVal = 2.0 + h ** 2.0 * q (xValues.[i])
    let fValue = h ** 2.0 * bigF (xValues.[i])
    alpha.[i + 1] <- bVal / (cVal - aVal * alpha.[i])
    beta.[i + 1] <- (fValue + aVal * beta.[i]) / (cVal - aVal * alpha.[i])

for i in n - 1 .. -1 .. n / 2 do
    let cVal = 2.0 + h ** 2.0 * q (xValues.[i])
    let fValue = h ** 2.0 * bigF (xValues.[i])
    ksi.[i] <- aVal / (cVal - bVal * ksi.[i + 1])
    eta.[i] <- (fValue + bVal * eta.[i + 1]) / (cVal - bVal * ksi.[i + 1])

y.[n / 2] <-
    (beta.[n / 2 + 1] + alpha.[n / 2 + 1] * eta.[n / 2 + 1])
    / (1.0 - alpha.[n / 2 + 1] * ksi.[n / 2 + 1])

for i in n / 2 - 1 .. -1 .. 0 do
    y.[i] <- alpha.[i + 1] * y.[i + 1] + beta.[i + 1]

for i in n / 2 .. n - 1 do
    y.[i + 1] <- ksi.[i + 1] * y.[i] + eta.[i + 1]

let r = (uValues, y) ||> Seq.map2 (-) |> Seq.map abs |> Seq.toArray

printfn "Хакимов Артур вариант 29 встречная прогонка"
printfn "%-10s %-10s %-10s %-10s" "x" "y" "u" "r"

for i in 0..n do
    printfn "%-10.4f %-10.4f %-10.4f %-10.8f" xValues.[i] y.[i] uValues.[i] r.[i]
