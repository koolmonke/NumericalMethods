open C1.Lab1
open C2.Lab1.Integrate

open Functions.Expressions
open MatrixArithmetic

[<Literal>]
let variant = 19

[<Literal>]
let n = 3u

let x = Var "x"


let xLeft = 0.

let xRight = 1.5 - float variant / 25.

let muLeft = float variant / 2. - 1.
let muRight = 0.

let xLenght = xRight - xLeft

let k = x ** Val 0.5 + Val(float variant / 3.)

let p =
    (x + Val 4)
    / (x ** Val 2 + Val(float variant / 3.))

let f = x * Val(2. / (float variant + 1.))

let phi k =
    if k = 0. then
        let xCoeff = muLeft - muRight
        let freeCoeff = xLeft * muRight - xRight * muLeft
        ((Val xCoeff) * x + Val freeCoeff) / Val xLenght
    else
        (Val xRight - x) * (x - Val xLeft) ** Val k


let lagrange i j =
    let inIntegral =
        k * diff (phi i) "x" * diff (phi j) "x"
        + p * phi i * phi j

    integral (xLeft, xRight, n) (evalf inIntegral "x")

let b q =
    integral
        (xLeft, xRight, n)
        (evalf
            (f * phi q
             - p * phi 0 * phi q
             - k * diff (phi 0) "x" * diff (phi q) "x")
            "x")


let a =
    Matrix(int n, int n)
        .From(
            seq {
                for i in 1 .. int n do
                    for j in 1 .. int n do
                        yield lagrange j i
            }
        )

printfn $"{a}"

let bVector =
    Vector(seq { 1.0 .. (float n) } |> Seq.map b)


let gaussSolution = GaussSolver(a, bVector).SolutionVector


printfn $"{gaussSolution}"

let solution =
    (seq { for i in 1 .. (int n) -> phi (float i) * Val gaussSolution.[int (i - 1)] })
    |> Seq.reduce (+)
    |> (+) (phi 0)
    |> (*) (Val -1.)

printfn $"{show solution}"

printfn "%A %A" xLeft muLeft

for i in xLeft .. xLenght / 4.0 .. xRight do
    printfn "%A %A" i (evalf solution "x" i)

printfn "%A %A" xRight muRight
