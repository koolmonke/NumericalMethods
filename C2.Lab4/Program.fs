open C2.Lab4

[<Literal>]
let var = 19

[<Literal>]
let T = 1.

[<Literal>]
let h = 0.1

let alpha = 2. + 0.5 * float var

let n = 1 + int (T / h)

let f1 t y1 y2 = sin (alpha * y1 ** 2.) + t + y2

let f2 t y1 y2 = t + y1 - alpha * y2 ** 2. + 1.

let f12 t y1 y2 = f1 t y1 y2, f2 t y1 y2


printfn "t     y1       y2       y3       y4"

for t, (y1, y2), (y3, y4) in
    Seq.zip3 (seq { 0.0 .. 0.1 .. 1.0 }) (Hoyna.solve f1 f2 n h (1., 0.5)) (Euler.solve f1 f2 n h (1., 0.5)) do
    printfn $"%.1f{t} %8.4f{y1} %8.4f{y2} %8.4f{y3} %8.4f{y4}"
